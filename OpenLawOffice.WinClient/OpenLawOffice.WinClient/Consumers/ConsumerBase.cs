﻿using System;
using System.Collections.Generic;
using OpenLawOffice.Common.Rest.Responses;
using RestSharp;

namespace OpenLawOffice.WinClient.Consumers
{
    public abstract class ConsumerBase<TRequest, TResponse>
        where TRequest : OpenLawOffice.Common.Rest.Requests.RequestBase
        where TResponse : ResponseBase
    {
        public abstract string Resource { get; }

        private string BuildFullResource(TRequest request)
        {
            if (typeof(Common.Rest.Requests.IHasIntId).IsAssignableFrom(typeof(TRequest)))
            {
                int? id = ((Common.Rest.Requests.IHasIntId)request).Id;
                if (!id.HasValue) throw new InvalidServiceTargetException("A GetSingle request must have an Id.");
                return (Resource + id.Value.ToString());
            }
            else if (typeof(Common.Rest.Requests.IHasLongId).IsAssignableFrom(typeof(TRequest)))
            {
                long? id = ((Common.Rest.Requests.IHasLongId)request).Id;
                if (!id.HasValue) throw new InvalidServiceTargetException("A GetSingle request must have an Id.");
                return (Resource + id.Value.ToString());
            }
            else if (typeof(Common.Rest.Requests.IHasGuidId).IsAssignableFrom(typeof(TRequest)))
            {
                Guid? id = ((Common.Rest.Requests.IHasGuidId)request).Id;
                if (!id.HasValue) throw new InvalidServiceTargetException("A GetSingle request must have an Id.");
                return (Resource + id.Value.ToString());
            }
            else
            {
                // Can't determine, so just fire it at the service and let the service figure it out
                // Technically we could just do this for all of them, but that seems a bit lazy.
                return Resource;
            }
        }

        private RestRequestAsyncHandle Execute(RestRequest restSharpRequest, TRequest request, Action<ConsumerResult<TRequest, TResponse>> callback)
        {
            RestClient client = new RestClient(Globals.Instance.Settings.HostBaseUrl);

            return client.ExecuteAsync<Common.Rest.Responses.ResponseContainer<TResponse>>(restSharpRequest, (response, handle) =>
            {
                if (callback != null)
                {
                    if (response.Data != null)
                        callback(new ConsumerResult<TRequest, TResponse>()
                        {
                            //Sender = this,
                            Request = request,
                            Response = response.Data.Data,
                            RestSharpResponse = response,
                            Handle = handle
                        });
                    else
                        callback(new ConsumerResult<TRequest, TResponse>()
                        {
                            //Sender = this,
                            Request = request,
                            Response = null,
                            RestSharpResponse = response,
                            Handle = handle
                        });
                }
            });
        }

        public virtual RestRequestAsyncHandle GetSingle(TRequest request, Action<ConsumerResult<TRequest, TResponse>> callback)
        {
            RestRequest restRequest = RequestBuilder.Build(BuildFullResource(request), Method.GET, request);
            return Execute(restRequest, request, callback);
        }

        public virtual RestRequestAsyncHandle GetList(TRequest request, Action<ConsumerResult<TRequest, List<TResponse>>> callback)
        {
            RestClient client = new RestClient(Globals.Instance.Settings.HostBaseUrl);
            RestRequest restRequest = RequestBuilder.Build(Resource, Method.GET, request);

            return client.ExecuteAsync<Common.Rest.Responses.ListResponseContainer<TResponse>>(restRequest, (response, handle) =>
            {
                if (callback != null)
                {
                    if (response.Data != null)
                        callback(new ConsumerResult<TRequest, List<TResponse>>()
                        {
                            //Sender = this,
                            Request = request,
                            Response = response.Data.Data,
                            RestSharpResponse = response,
                            Handle = handle
                        });
                    else
                        callback(new ConsumerResult<TRequest, List<TResponse>>()
                        {
                            //Sender = this,
                            Request = request,
                            Response = null,
                            RestSharpResponse = response,
                            Handle = handle
                        });
                }
            });
        }

        public virtual RestRequestAsyncHandle Create(TRequest request, Action<ConsumerResult<TRequest, TResponse>> callback)
        {
            RestRequest restRequest = RequestBuilder.Build(Resource, Method.POST, request);
            return Execute(restRequest, request, callback);
        }

        public virtual RestRequestAsyncHandle Update(TRequest request, Action<ConsumerResult<TRequest, TResponse>> callback)
        {
            RestRequest restRequest = RequestBuilder.Build(Resource, Method.PUT, request);
            return Execute(restRequest, request, callback);
        }

        public virtual RestRequestAsyncHandle Disable(TRequest request, Action<ConsumerResult<TRequest, TResponse>> callback)
        {
            RestRequest restRequest = RequestBuilder.Build(Resource, Method.DELETE, request);
            return Execute(restRequest, request, callback);
        }
    }
}