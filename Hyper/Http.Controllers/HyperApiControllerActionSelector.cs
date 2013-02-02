/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// Represents a reflection based action selector.
    /// </summary>
    public class HyperApiControllerActionSelector : IHttpActionSelector
    {
        private readonly object _cacheKey = new object();

        private ActionSelectorCacheItem _fastCache;

        /// <summary>
        /// Gets the action mappings for the <see cref="T:Hyper.Http.Controllers.HyperApiControllerActionSelector" />.
        /// </summary>
        /// <param name="controllerDescriptor">The information that describes a controller.</param>
        /// <returns>
        /// The action mappings.
        /// </returns>
        public virtual ILookup<string, HttpActionDescriptor> GetActionMapping(
            HttpControllerDescriptor controllerDescriptor)
        {
            if (controllerDescriptor == null)
            {
                throw new ArgumentNullException("controllerDescriptor");
            }

            return GetInternalSelector(controllerDescriptor).GetActionMapping();
        }

        /// <summary>
        /// Selects an action for the <see cref="T:Hyper.Http.Controllers.HyperApiControllerActionSelector"/>.
        /// </summary>
        /// <returns>
        /// The selected action.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param>
        public virtual HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            return GetInternalSelector(controllerContext.ControllerDescriptor).SelectAction(controllerContext);
        }

        /// <summary>
        /// Gets the internal selector.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns></returns>
        private ActionSelectorCacheItem GetInternalSelector(HttpControllerDescriptor controllerDescriptor)
        {
            if (_fastCache == null)
            {
                var selectorCacheItem = new ActionSelectorCacheItem(controllerDescriptor);
                Interlocked.CompareExchange(ref _fastCache, selectorCacheItem, null);
                return selectorCacheItem;
            }
            if (_fastCache.HttpControllerDescriptor == controllerDescriptor)
            {
                return _fastCache;
            }
            return
                (ActionSelectorCacheItem)
                controllerDescriptor.Properties.GetOrAdd(
                    _cacheKey, _ => (object)new ActionSelectorCacheItem(controllerDescriptor));
        }

        /// <summary>
        /// ActionSelectorCacheItem class.
        /// </summary>
        private class ActionSelectorCacheItem
        {
            private readonly ReflectedHttpActionDescriptor[] _actionDescriptors;

            private readonly ILookup<string, ReflectedHttpActionDescriptor> _actionNameMapping;

            private readonly IDictionary<ReflectedHttpActionDescriptor, string[]> _actionParameterNames =
                new Dictionary<ReflectedHttpActionDescriptor, string[]>();

            private readonly HttpMethod[] _cacheListVerbKinds = new HttpMethod[3]
                { HttpMethod.Get, HttpMethod.Put, HttpMethod.Post };

            private readonly ReflectedHttpActionDescriptor[][] _cacheListVerbs;

            private readonly HttpControllerDescriptor _controllerDescriptor;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionSelectorCacheItem" /> class.
            /// </summary>
            /// <param name="controllerDescriptor">The controller descriptor.</param>
            public ActionSelectorCacheItem(HttpControllerDescriptor controllerDescriptor)
            {
                _controllerDescriptor = controllerDescriptor;
                var all =
                    Array.FindAll(
                        _controllerDescriptor.ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public),
                        IsValidActionMethod);
                
                _actionDescriptors = new ReflectedHttpActionDescriptor[all.Length];
                for (int index = 0; index < all.Length; ++index)
                {
                    var key = new ReflectedHttpActionDescriptor(_controllerDescriptor, all[index]);
                    _actionDescriptors[index] = key;
                    var actionBinding = key.ActionBinding;
                    _actionParameterNames.Add(
                        key,
                        actionBinding.ParameterBindings.Where(
                            binding =>
                                {
                                    if (!binding.Descriptor.IsOptional
                                        && TypeHelper.IsSimpleUnderlyingType(binding.Descriptor.ParameterType))
                                    {
                                        return binding.WillReadUri();
                                    }
                                    return false;
                                }).Select(binding => binding.Descriptor.Prefix ?? binding.Descriptor.ParameterName).
                            ToArray());
                }

                _actionNameMapping = _actionDescriptors.ToLookup(actionDesc => actionDesc.ActionName, StringComparer.OrdinalIgnoreCase);
                var length = _cacheListVerbKinds.Length;
                _cacheListVerbs = new ReflectedHttpActionDescriptor[length][];
                for (int index = 0; index < length; ++index)
                {
                    _cacheListVerbs[index] = FindActionsForVerbWorker(_cacheListVerbKinds[index]);
                }
            }

            /// <summary>
            /// Gets the HTTP controller descriptor.
            /// </summary>
            /// <value>
            /// The HTTP controller descriptor.
            /// </value>
            public HttpControllerDescriptor HttpControllerDescriptor
            {
                get
                {
                    return _controllerDescriptor;
                }
            }

            /// <summary>
            /// Gets the action mapping.
            /// </summary>
            /// <returns></returns>
            public ILookup<string, HttpActionDescriptor> GetActionMapping()
            {
                return new LookupAdapter { Source = _actionNameMapping };
            }

            /// <summary>
            /// Selects the action.
            /// </summary>
            /// <param name="controllerContext">The controller context.</param>
            /// <returns></returns>
            /// <exception cref="System.Web.Http.HttpResponseException"></exception>
            public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
            {
                string index;
                bool hasActionRouteKey = DictionaryExtensions.TryGetValue<string>(
                    controllerContext.RouteData.Values, "action", out index);
                HttpMethod incomingMethod = controllerContext.Request.Method;
                ReflectedHttpActionDescriptor[] actionDescriptorArray1;
                if (hasActionRouteKey)
                {
                    ReflectedHttpActionDescriptor[] actionDescriptorArray2 = _actionNameMapping[index].ToArray();
                    if (actionDescriptorArray2.Length == 0)
                    {
                        throw new HttpResponseException(
                            HttpRequestMessageExtensions.CreateErrorResponse(
                                controllerContext.Request,
                                HttpStatusCode.NotFound,
                                string.Format(
                                    "ResourceNotFound {0}", new object[1] { controllerContext.Request.RequestUri }),
                                string.Format(
                                    "ApiControllerActionSelector_ActionNameNotFound {0} {1}",
                                    _controllerDescriptor.ControllerName,
                                    index)));
                    }
                    else
                    {
                        actionDescriptorArray1 =
                            actionDescriptorArray2.Where(
                                actionDescriptor => actionDescriptor.SupportedHttpMethods.Contains(incomingMethod)).
                                ToArray();
                    }
                }
                else
                {
                    actionDescriptorArray1 = FindActionsForVerb(incomingMethod);
                }
                if (actionDescriptorArray1.Length == 0)
                {
                    throw new HttpResponseException(
                        HttpRequestMessageExtensions.CreateErrorResponse(
                            controllerContext.Request,
                            HttpStatusCode.MethodNotAllowed,
                            string.Format(
                                "ApiControllerActionSelector_HttpMethodNotSupported",
                                new object[1] { incomingMethod })));
                }
                IEnumerable<ReflectedHttpActionDescriptor> andQueryParameters =
                    FindActionUsingRouteAndQueryParameters(controllerContext, actionDescriptorArray1, hasActionRouteKey);
                List<ReflectedHttpActionDescriptor> list = RunSelectionFilters(controllerContext, andQueryParameters);
                switch (list.Count)
                {
                    case 0:
                        throw new HttpResponseException(
                            HttpRequestMessageExtensions.CreateErrorResponse(
                                controllerContext.Request,
                                HttpStatusCode.NotFound,
                                string.Format(
                                    "ResourceNotFound",
                                    new object[1] { controllerContext.Request.RequestUri }),
                                string.Format(
                                    "ApiControllerActionSelector_ActionNotFound",
                                    new object[1] { _controllerDescriptor.ControllerName })));
                    case 1:
                        return list[0];
                    default:
                        throw new InvalidOperationException(
                            string.Format("ApiControllerActionSelector_AmbiguousMatch",
                            new object[1] { CreateAmbiguousMatchList(list) }));
                }
            }

            /// <summary>
            /// Creates the ambiguous match list.
            /// </summary>
            /// <param name="ambiguousDescriptors">The ambiguous descriptors.</param>
            /// <returns></returns>
            private static string CreateAmbiguousMatchList(IEnumerable<HttpActionDescriptor> ambiguousDescriptors)
            {
                var stringBuilder = new StringBuilder();
                foreach (ReflectedHttpActionDescriptor actionDescriptor in ambiguousDescriptors)
                {
                    MethodInfo methodInfo = actionDescriptor.MethodInfo;
                    stringBuilder.AppendLine();
                    stringBuilder.Append(
                        string.Format(
                            "ActionSelector_AmbiguousMatchType {0} {1}", methodInfo, methodInfo.DeclaringType.FullName));
                }
                return stringBuilder.ToString();
            }

            /// <summary>
            /// Determines whether the specified action parameters is subset.
            /// </summary>
            /// <param name="actionParameters">The action parameters.</param>
            /// <param name="routeAndQueryParameters">The route and query parameters.</param>
            /// <returns>
            ///   <c>true</c> if the specified action parameters is subset; otherwise, <c>false</c>.
            /// </returns>
            private static bool IsSubset(string[] actionParameters, HashSet<string> routeAndQueryParameters)
            {
                foreach (string str in actionParameters)
                {
                    if (!routeAndQueryParameters.Contains(str))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Determines whether [is valid action method] [the specified method info].
            /// </summary>
            /// <param name="methodInfo">The method info.</param>
            /// <returns>
            ///   <c>true</c> if [is valid action method] [the specified method info]; otherwise, <c>false</c>.
            /// </returns>
            private static bool IsValidActionMethod(MethodInfo methodInfo)
            {
                return !methodInfo.IsSpecialName
                       && !methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(TypeHelper.ApiControllerType);
            }

            /// <summary>
            /// Runs the selection filters.
            /// </summary>
            /// <param name="controllerContext">The controller context.</param>
            /// <param name="descriptorsFound">The descriptors found.</param>
            /// <returns></returns>
            private static List<ReflectedHttpActionDescriptor> RunSelectionFilters(
                HttpControllerContext controllerContext, IEnumerable<HttpActionDescriptor> descriptorsFound)
            {
                List<ReflectedHttpActionDescriptor> list1 = null;
                var list2 = new List<ReflectedHttpActionDescriptor>();
                using (IEnumerator<HttpActionDescriptor> enumerator = descriptorsFound.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var actionDescriptor = (ReflectedHttpActionDescriptor)enumerator.Current;
                        IActionMethodSelector[] iactionMethodSelector = actionDescriptor.CacheAttrsIActionMethodSelector;
                        if (iactionMethodSelector.Length == 0)
                        {
                            list2.Add(actionDescriptor);
                        }
                        else if (Array.TrueForAll(
                            iactionMethodSelector,
                            (selector => selector.IsValidForRequest(controllerContext, actionDescriptor.MethodInfo))))
                        {
                            if (list1 == null)
                            {
                                list1 = new List<ReflectedHttpActionDescriptor>();
                            }
                            list1.Add(actionDescriptor);
                        }
                    }
                }
                if (list1 != null && list1.Count > 0)
                {
                    return list1;
                }
                else
                {
                    return list2;
                }
            }

            /// <summary>
            /// Finds the action using route and query parameters.
            /// </summary>
            /// <param name="controllerContext">The controller context.</param>
            /// <param name="actionsFound">The actions found.</param>
            /// <param name="hasActionRouteKey">if set to <c>true</c> [has action route key].</param>
            /// <returns></returns>
            private IEnumerable<ReflectedHttpActionDescriptor> FindActionUsingRouteAndQueryParameters(
                HttpControllerContext controllerContext,
                IEnumerable<ReflectedHttpActionDescriptor> actionsFound,
                bool hasActionRouteKey)
            {
                var hashSet = new HashSet<string>(
                    controllerContext.RouteData.Values.Keys, StringComparer.OrdinalIgnoreCase);
                hashSet.Remove("controller");
                if (hasActionRouteKey)
                {
                    hashSet.Remove("action");
                }
                HttpRequestMessage request = controllerContext.Request;
                Uri requestUri = request.RequestUri;
                bool flag = requestUri != null && !string.IsNullOrEmpty(requestUri.Query);
                if (hashSet.Count != 0 || flag)
                {
                    var combinedParameterNames = new HashSet<string>(hashSet, StringComparer.OrdinalIgnoreCase);
                    if (flag)
                    {
                        foreach (KeyValuePair<string, string> keyValuePair in request.GetQueryNameValuePairs())
                        {
                            combinedParameterNames.Add(keyValuePair.Key);
                        }
                    }
                    actionsFound =
                        actionsFound.Where(
                            (descriptor => IsSubset(_actionParameterNames[descriptor], combinedParameterNames)));
                    if (actionsFound.Count() > 1)
                    {
                        actionsFound =
                            (actionsFound.GroupBy((descriptor => _actionParameterNames[descriptor].Length)).
                                OrderByDescending((g => g.Key))).First();
                    }
                }
                else
                {
                    actionsFound = actionsFound.Where((descriptor => _actionParameterNames[descriptor].Length == 0));
                }
                return actionsFound;
            }

            /// <summary>
            /// Finds the actions for verb.
            /// </summary>
            /// <param name="verb">The verb.</param>
            /// <returns></returns>
            private ReflectedHttpActionDescriptor[] FindActionsForVerb(HttpMethod verb)
            {
                for (int index = 0; index < _cacheListVerbKinds.Length; ++index)
                {
                    if (ReferenceEquals(verb, _cacheListVerbKinds[index]))
                    {
                        return _cacheListVerbs[index];
                    }
                }
                return FindActionsForVerbWorker(verb);
            }

            /// <summary>
            /// Finds the actions for verb worker.
            /// </summary>
            /// <param name="verb">The verb.</param>
            /// <returns></returns>
            private ReflectedHttpActionDescriptor[] FindActionsForVerbWorker(HttpMethod verb)
            {
                var list = new List<ReflectedHttpActionDescriptor>();
                foreach (ReflectedHttpActionDescriptor actionDescriptor in _actionDescriptors)
                {
                    if (actionDescriptor.SupportedHttpMethods.Contains(verb))
                    {
                        list.Add(actionDescriptor);
                    }
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// LookupAdapter class
        /// </summary>
        private class LookupAdapter : ILookup<string, HttpActionDescriptor>
        {
            public ILookup<string, ReflectedHttpActionDescriptor> Source;

            /// <summary>
            /// Gets the number of key/value collection pairs in the <see cref="T:System.Linq.ILookup`2" />.
            /// </summary>
            /// <returns>The number of key/value collection pairs in the <see cref="T:System.Linq.ILookup`2" />.</returns>
            public int Count
            {
                get
                {
                    return Source.Count;
                }
            }

            /// <summary>
            /// Gets the <see cref="IEnumerable" /> with the specified key.
            /// </summary>
            /// <value>
            /// The <see cref="IEnumerable" />.
            /// </value>
            /// <param name="key">The key.</param>
            /// <returns></returns>
            public IEnumerable<HttpActionDescriptor> this[string key]
            {
                get
                {
                    return Source[key];
                }
            }

            /// <summary>
            /// Determines whether a specified key exists in the <see cref="T:System.Linq.ILookup`2" />.
            /// </summary>
            /// <param name="key">The key to search for in the <see cref="T:System.Linq.ILookup`2" />.</param>
            /// <returns>
            /// true if <paramref name="key" /> is in the <see cref="T:System.Linq.ILookup`2" />; otherwise, false.
            /// </returns>
            public bool Contains(string key)
            {
                return Source.Contains(key);
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<IGrouping<string, HttpActionDescriptor>> GetEnumerator()
            {
                return Source.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return Source.GetEnumerator();
            }
        }
    }
}
*/