﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.Plugins.Douban
{
    public enum MediaType
    {
        movie,
        tv
    }
    public interface IDoubanClient
    {
        /// <summary>
        /// Gets one movie or tv item by doubanID.
        /// </summary>
        /// <param name="doubanID">The subject ID in Douban.</param>
        /// <param name="type">Subject type.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The subject of one item.</returns>
        public Task<Response.Subject> GetSubject(string doubanID, MediaType type,
            CancellationToken cancellationToken);
        public Task<Response.SubjectCredits> GetSubjectCredits(string doubanID, MediaType type, CancellationToken cancellationToken);
        public Task<Response.ElessarSubject> getPersonInfo(string doubanID, CancellationToken cancellationToken);
        /// <summary>
        /// Search in Douban by a search query.
        /// </summary>
        /// <param name="name">The content of search query.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>The Search Result.</returns>
        public Task<Response.SearchResult> Search(string name, CancellationToken cancellationToken);

        /// <summary>
        /// Simply gets the response by HTTP without any other options.
        /// </summary>
        /// <param name="url">Request URL.</param>
        /// <param name="cancellationToken">Used to cancel the request.</param>
        /// <returns>Simple Http Response.</returns>
        public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken);

    }
}