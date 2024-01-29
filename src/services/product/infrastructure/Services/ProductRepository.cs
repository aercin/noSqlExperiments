using application.Abstractions;
using application.Features.Queries.Models;
using Couchbase.Core.Exceptions;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Couchbase.Query;
using domain.Entities;
using Microsoft.Extensions.Configuration;

namespace infrastructure.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IBucketProvider _bucketProvider;
        private readonly IConfiguration _configuration;

        public ProductRepository(IBucketProvider bucketProvider, IConfiguration config)
        {
            this._bucketProvider = bucketProvider;
            this._configuration = config;
        }

        public async Task<Product> AddAsync(Product product)
        {
            var bucket = await this._bucketProvider.GetBucketAsync(this._configuration.GetValue<string>("Couchbase:Bucket"));
            var scope = await bucket.ScopeAsync(this._configuration.GetValue<string>("Couchbase:Scope"));
            var collection = await scope.CollectionAsync(this._configuration.GetValue<string>("Couchbase:Collection"));
            
            try
            {
                var id = Guid.NewGuid();
                var result = await collection.InsertAsync(id.ToString(), product);
                product.Cas = result.Cas.ToString();
                product.Id = id;
            }
            catch (DocumentExistsException ex)
            {
                //loglama konulabilir.
            }

            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var bucket = await this._bucketProvider.GetBucketAsync(this._configuration.GetValue<string>("Couchbase:Bucket"));
            var scope = await bucket.ScopeAsync(this._configuration.GetValue<string>("Couchbase:Scope"));
            var collection = await scope.CollectionAsync(this._configuration.GetValue<string>("Couchbase:Collection"));
            bool result = true;
            try
            { 
                var replaceOptions = new ReplaceOptions();
                replaceOptions.Cas(Convert.ToUInt64(product.Cas));

                await collection.ReplaceAsync(product.Id.ToString(), product, replaceOptions);
            }
            catch (CasMismatchException ex)
            {
                //loglama konulabilir.
                result = false;
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Guid productId, string cas)
        {
            var bucket = await this._bucketProvider.GetBucketAsync(this._configuration.GetValue<string>("Couchbase:Bucket"));
            var scope = await bucket.ScopeAsync(this._configuration.GetValue<string>("Couchbase:Scope"));
            var collection = await scope.CollectionAsync(this._configuration.GetValue<string>("Couchbase:Collection"));
            bool result = true;
            try
            {
                var removeOptions = new RemoveOptions();
                removeOptions.Cas(Convert.ToUInt64(cas));

                await collection.RemoveAsync(productId.ToString(), removeOptions);
            }
            catch (CasMismatchException ex)
            {
                //loglama konulabilir.
                result = false;
            }

            return result;
        }

        public async Task<Product?> GetAsync(Guid productId)
        {
            var bucket = await this._bucketProvider.GetBucketAsync(this._configuration.GetValue<string>("Couchbase:Bucket"));
            var scope = await bucket.ScopeAsync(this._configuration.GetValue<string>("Couchbase:Scope"));
            var collection = await scope.CollectionAsync(this._configuration.GetValue<string>("Couchbase:Collection"));

            var queryResult = await collection.GetAsync(productId.ToString());
            var result = queryResult?.ContentAs<Product>();
            if (result != null)
            {
                result.Id = productId;
                result.Cas = queryResult.Cas.ToString();
            }
            return result;
        }

        public async Task<List<Product>> GetAsync(Action<ProductQueryOptions> options)
        {
            var bucket = await this._bucketProvider.GetBucketAsync(this._configuration.GetValue<string>("Couchbase:Bucket"));
            var cluster = bucket.Cluster;

            var queryCollectionPart = $"`{this._configuration.GetValue<string>("Couchbase:Bucket")}`.`{this._configuration.GetValue<string>("Couchbase:Scope")}`.`{this._configuration.GetValue<string>("Couchbase:Collection")}`";

            var filter = new ProductQueryOptions();
            options(filter);

            var queryOptions = new QueryOptions();
            var queryFilterPart = string.Empty;
            if (filter.CategoryId > 0)
            {
                queryFilterPart = " AND c.CategoryId=$categoryId";
                queryOptions.Parameter("$categoryId", filter.CategoryId);
            }
            if (!string.IsNullOrEmpty(filter.Code))
            {
                queryFilterPart = " AND c.Code=$code";
                queryOptions.Parameter("$code", filter.Code);
            }
            if (!string.IsNullOrEmpty(queryFilterPart))
            {
                queryFilterPart = @$"WHERE 1=1 {queryFilterPart}";
            }

            var query = @$"
                            SELECT data.*,META(data).id,META(data).cas
                            FROM {queryCollectionPart} AS data 
                            {queryFilterPart}
                            ORDER BY META(data).id 
                            LIMIT {filter.PageSize} 
                            OFFSET {(filter.PageNumber - 1) * filter.PageSize}
                        ";


            var result = await cluster.QueryAsync<Product>(query, queryOptions);

            return result.Rows.ToListAsync().Result;
        }
    }
}
