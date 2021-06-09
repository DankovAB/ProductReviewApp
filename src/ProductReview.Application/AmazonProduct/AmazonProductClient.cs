using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using ProductReviewApp.Application.AmazonProduct.Models;

namespace ProductReviewApp.Application.AmazonProduct
{
    public class AmazonProductClient: IProductClient
    {
        private readonly HttpClient _httpClient;
        private readonly string sortByRecentReview = "ref=cm_cr_arp_d_viewopt_srt?sortBy=recent&pageNumber=1";
        private readonly ILogger<AmazonProductClient> _logger;

        public AmazonProductClient(HttpClient httpClient, ILogger<AmazonProductClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductSnapshot> GetProductWithReviews(string asin, DateTime? lastReviewsDate = null)
        {
            if (string.IsNullOrWhiteSpace(asin))
                throw new ArgumentNullException(nameof(asin));

            var asinUrl = $"{asin}/{sortByRecentReview}";
            var document = await LoadDocument(asinUrl);

            var productSnapshot = ParseProduct(document);
            productSnapshot.Asin = asin;

            var (reviewSnapshots, nextPageUrl) = ParseReviews(document, lastReviewsDate);
            productSnapshot.ProductReviews.AddRange(reviewSnapshots);

            while (nextPageUrl != null)
            {
                document = await LoadDocument(nextPageUrl);

                (reviewSnapshots, nextPageUrl) = ParseReviews(document, lastReviewsDate);
                productSnapshot.ProductReviews.AddRange(reviewSnapshots);
            }

            return productSnapshot;
        }

        private async Task<IDocument> LoadDocument(string url)
        {
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStringAsync();

            return await CreateDocument(source);
        }

        private ProductSnapshot ParseProduct(IDocument document)
        {
            var snapshot = new ProductSnapshot
            {
                Link = document.QuerySelector("div.a-row.product-title > h1 > a")?.Attributes["href"]?.Value
            };

            return snapshot;
        }

        private static async Task<IDocument> CreateDocument(string source)
        {
            //Use the default configuration for AngleSharp
            var config = Configuration.Default;

            //Create a new context for evaluating webpages with the given config
            var context = BrowsingContext.New(config);

            //Create a virtual request to specify the document to load (here from our fixed string)
            var document = await context.OpenAsync(req => req.Content(source));
            return document;
        }

        private (IReadOnlyCollection<ProductReviewSnapshot> reviews, string nextPageUrl) ParseReviews(IDocument document, DateTime? lastDateReview)
        {
            var startRatingClass = "a-star-";
            string nextPageUrl = null;
            var reviewSnapshots = new List<ProductReviewSnapshot>();

            var reviewElements = document.QuerySelectorAll(".review");
            var stopCrawling = false;

            foreach (var reviewElement in reviewElements)
            {
                var reviewSnapshot = ParseReview(reviewElement, startRatingClass);

                if (reviewSnapshot.Date < lastDateReview)
                {
                    stopCrawling = true;
                    break;
                }

                reviewSnapshots.Add(reviewSnapshot);
            }

            if (!stopCrawling)
            {
                var nextPageLink = document.QuerySelector(".a-pagination > li.a-last > a");
                if (nextPageLink != null)
                {
                    nextPageUrl = nextPageLink.GetAttribute("href");
                }
            }

            return (reviewSnapshots, nextPageUrl);
        }

        private ProductReviewSnapshot ParseReview(IElement reviewElement, string startRatingClass)
        {
            var idRaw = reviewElement.GetAttribute("id");
            var titleRaw = reviewElement.QuerySelector(".review-title > span").TextContent.Trim();
            var dateRaw = reviewElement.QuerySelector(".review-date").TextContent.Trim();
            var profileNameRaw = reviewElement.QuerySelector(".a-profile-name").TextContent.Trim();
            var starRaw = reviewElement.QuerySelector(".review-rating").ClassList
                .FirstOrDefault(r => r.Contains(startRatingClass));
            var reviewContentRaw = reviewElement.QuerySelector(".review-text-content > span").TextContent.Trim();

            var date = ParseDateTime(dateRaw);

            var starValue = ParseStarValue(starRaw, startRatingClass);

            return new ProductReviewSnapshot
            {
                Id = idRaw,
                Title = titleRaw,
                ProfileName = profileNameRaw,
                Review = reviewContentRaw,
                Date = date,
                Rating = starValue
            };
        }

        private int ParseStarValue(string starRaw, string startRatingClass)
        {
            var starValue = -1;
            starRaw = starRaw?.Replace(startRatingClass, "");
            if (string.IsNullOrWhiteSpace(starRaw) || !int.TryParse(starRaw, out starValue))
            {
                _logger.LogWarning($"cannot convert '{starRaw}' to rating");
            }

            return starValue;
        }

        private DateTime ParseDateTime(string dateRaw)
        {
            DateTime date = DateTime.MinValue;
            var match = Regex.Match(dateRaw, @"(?<=on )[A-S].+[12]\d{3}$");
            if (match.Success)
            {
                if (!DateTime.TryParseExact(match.Value, "MMMM d, yyyy",
                    System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                    out date))
                {
                    _logger.LogWarning($"cannot convert '{match.Value}' to date");
                }
            }
            else
            {
                _logger.LogWarning($"cannot convert '{dateRaw}' to date");
            }

            return date;
        }
    }
}
