﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Net.Http.Formatting;
using System.Net.Http.Formatting.Mocks;
using System.Net.Http.Headers;
using System.Net.Http.Mocks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TestCommon;
using Moq;

namespace System.Net.Http
{
    public class HttpClientExtensionsTest
    {
        private readonly MediaTypeFormatter _formatter = new MockMediaTypeFormatter { CallBase = true };
        private readonly HttpClient _client;
        private readonly MediaTypeHeaderValue _mediaTypeHeader = MediaTypeHeaderValue.Parse("foo/bar; charset=utf-16");

        public HttpClientExtensionsTest()
        {
            Mock<TestableHttpMessageHandler> handlerMock = new Mock<TestableHttpMessageHandler> { CallBase = true };
            handlerMock
                .Setup(h => h.SendAsyncPublic(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns((HttpRequestMessage request, CancellationToken _) => Task.FromResult(new HttpResponseMessage() { RequestMessage = request }));

            _client = new HttpClient(handlerMock.Object);
        }

        [Fact]
        public void PostAsJsonAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsJsonAsync("http://www.example.com", new object()), "client");
        }

        [Fact]
        public void PostAsJsonAsync_String_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsJsonAsync((string)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsJsonAsync_String_UsesJsonMediaTypeFormatter()
        {
            var result = _client.PostAsJsonAsync("http://example.com", new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<JsonMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PostAsXmlAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsXmlAsync("http://www.example.com", new object()), "client");
        }

        [Fact]
        public void PostAsXmlAsync_String_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsXmlAsync((string)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsXmlAsync_String_UsesXmlMediaTypeFormatter()
        {
            var result = _client.PostAsXmlAsync("http://example.com", new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<XmlMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PostAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsync("http://www.example.com", new object(), new JsonMediaTypeFormatter(), "text/json"), "client");
        }

        [Fact]
        public void PostAsync_String_WhenRequestUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsync((string)null, new object(), new JsonMediaTypeFormatter(), "text/json"),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsync_String_WhenRequestUriIsSet_CreatesRequestWithAppropriateUri()
        {
            _client.BaseAddress = new Uri("http://example.com/");

            var result = _client.PostAsync("myapi/", new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Equal("http://example.com/myapi/", request.RequestUri.ToString());
        }

        [Fact]
        public void PostAsync_String_WhenAuthoritativeMediaTypeIsSet_CreatesRequestWithAppropriateContentType()
        {
            var result = _client.PostAsync("http://example.com/myapi/", new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
            Assert.Equal("utf-16", request.Content.Headers.ContentType.CharSet);
        }

        [Fact]
        public void PostAsync_String_WhenAuthoritativeMediaTypeStringIsSet_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PostAsync("http://example.com/myapi/", new object(), _formatter, mediaType, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PostAsync_String_WhenAuthoritativeMediaTypeStringIsSetWithoutCT_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PostAsync("http://example.com/myapi/", new object(), _formatter, mediaType);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PostAsync_String_WhenFormatterIsSet_CreatesRequestWithObjectContentAndCorrectFormatter()
        {
            var result = _client.PostAsync("http://example.com/myapi/", new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            var content = Assert.IsType<ObjectContent<object>>(request.Content);
            Assert.Same(_formatter, content.Formatter);
        }

        [Fact]
        public void PostAsync_String_IssuesPostRequest()
        {
            var result = _client.PostAsync("http://example.com/myapi/", new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Same(HttpMethod.Post, request.Method);
        }

        [Fact]
        public void PostAsync_String_WhenMediaTypeFormatterIsNull_ThrowsException()
        {
            Assert.ThrowsArgumentNull(() => _client.PostAsync("http://example.com", new object(), formatter: null), "formatter");
        }

        [Fact]
        public void PutAsJsonAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsJsonAsync("http://www.example.com", new object()), "client");
        }

        [Fact]
        public void PutAsJsonAsync_String_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsJsonAsync((string)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsJsonAsync_String_UsesJsonMediaTypeFormatter()
        {
            var result = _client.PutAsJsonAsync("http://example.com", new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<JsonMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PutAsXmlAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsXmlAsync("http://www.example.com", new object()), "client");
        }

        [Fact]
        public void PutAsXmlAsync_String_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsXmlAsync((string)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsXmlAsync_String_UsesXmlMediaTypeFormatter()
        {
            var result = _client.PutAsXmlAsync("http://example.com", new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<XmlMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PutAsync_String_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsync("http://www.example.com", new object(), new JsonMediaTypeFormatter(), "text/json"), "client");
        }

        [Fact]
        public void PutAsync_String_WhenRequestUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsync((string)null, new object(), new JsonMediaTypeFormatter(), "text/json"),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsync_String_WhenRequestUriIsSet_CreatesRequestWithAppropriateUri()
        {
            _client.BaseAddress = new Uri("http://example.com/");

            var result = _client.PutAsync("myapi/", new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Equal("http://example.com/myapi/", request.RequestUri.ToString());
        }

        [Fact]
        public void PutAsync_String_WhenAuthoritativeMediaTypeIsSet_CreatesRequestWithAppropriateContentType()
        {
            var result = _client.PutAsync("http://example.com/myapi/", new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
            Assert.Equal("utf-16", request.Content.Headers.ContentType.CharSet);
        }

        [Fact]
        public void PutAsync_String_WhenFormatterIsSet_CreatesRequestWithObjectContentAndCorrectFormatter()
        {
            var result = _client.PutAsync("http://example.com/myapi/", new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            var content = Assert.IsType<ObjectContent<object>>(request.Content);
            Assert.Same(_formatter, content.Formatter);
        }

        [Fact]
        public void PutAsync_String_WhenAuthoritativeMediaTypeStringIsSet_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PutAsync("http://example.com/myapi/", new object(), _formatter, mediaType, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PutAsync_String_WhenAuthoritativeMediaTypeStringIsSetWithoutCT_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PutAsync("http://example.com/myapi/", new object(), _formatter, mediaType);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PutAsync_String_IssuesPutRequest()
        {
            var result = _client.PutAsync("http://example.com/myapi/", new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Same(HttpMethod.Put, request.Method);
        }

        [Fact]
        public void PutAsync_String_WhenMediaTypeFormatterIsNull_ThrowsException()
        {
            Assert.ThrowsArgumentNull(() => _client.PutAsync("http://example.com", new object(), formatter: null), "formatter");
        }

        [Fact]
        public void PostAsJsonAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsJsonAsync(new Uri("http://www.example.com"), new object()), "client");
        }

        [Fact]
        public void PostAsJsonAsync_Uri_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsJsonAsync((Uri)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsJsonAsync_Uri_UsesJsonMediaTypeFormatter()
        {
            var result = _client.PostAsJsonAsync(new Uri("http://example.com"), new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<JsonMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PostAsXmlAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsXmlAsync(new Uri("http://www.example.com"), new object()), "client");
        }

        [Fact]
        public void PostAsXmlAsync_Uri_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsXmlAsync((Uri)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsXmlAsync_Uri_UsesXmlMediaTypeFormatter()
        {
            var result = _client.PostAsXmlAsync(new Uri("http://example.com"), new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<XmlMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PostAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PostAsync(new Uri("http://www.example.com"), new object(), new JsonMediaTypeFormatter(), "text/json"), "client");
        }

        [Fact]
        public void PostAsync_Uri_WhenRequestUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PostAsync((Uri)null, new object(), new JsonMediaTypeFormatter(), "text/json"),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PostAsync_Uri_WhenRequestUriIsSet_CreatesRequestWithAppropriateUri()
        {
            _client.BaseAddress = new Uri("http://example.com/");

            var result = _client.PostAsync(new Uri("myapi/", UriKind.Relative), new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Equal("http://example.com/myapi/", request.RequestUri.ToString());
        }

        [Fact]
        public void PostAsync_Uri_WhenAuthoritativeMediaTypeIsSet_CreatesRequestWithAppropriateContentType()
        {
            var result = _client.PostAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
            Assert.Equal("utf-16", request.Content.Headers.ContentType.CharSet);
        }

        [Fact]
        public void PostAsync_Uri_WhenFormatterIsSet_CreatesRequestWithObjectContentAndCorrectFormatter()
        {
            var result = _client.PostAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            var content = Assert.IsType<ObjectContent<object>>(request.Content);
            Assert.Same(_formatter, content.Formatter);
        }

        [Fact]
        public void PostAsync_Uri_WhenAuthoritativeMediaTypeStringIsSet_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PostAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, mediaType, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PostAsync_Uri_WhenAuthoritativeMediaTypeStringIsSetWithoutCT_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PostAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, mediaType);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PostAsync_Uri_IssuesPostRequest()
        {
            var result = _client.PostAsync(new Uri("http://example.com/myapi/"), new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Same(HttpMethod.Post, request.Method);
        }

        [Fact]
        public void PostAsync_Uri_WhenMediaTypeFormatterIsNull_ThrowsException()
        {
            Assert.ThrowsArgumentNull(() => _client.PostAsync(new Uri("http://example.com"), new object(), formatter: null), "formatter");
        }

        [Fact]
        public void PutAsJsonAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsJsonAsync(new Uri("http://www.example.com"), new object()), "client");
        }

        [Fact]
        public void PutAsJsonAsync_Uri_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsJsonAsync((Uri)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsJsonAsync_Uri_UsesJsonMediaTypeFormatter()
        {
            var result = _client.PutAsJsonAsync(new Uri("http://example.com"), new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<JsonMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PutAsXmlAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsXmlAsync(new Uri("http://www.example.com"), new object()), "client");
        }

        [Fact]
        public void PutAsXmlAsync_Uri_WhenUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsXmlAsync((Uri)null, new object()),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsXmlAsync_Uri_UsesXmlMediaTypeFormatter()
        {
            var result = _client.PutAsXmlAsync(new Uri("http://example.com"), new object());

            var response = result.Result;
            var content = Assert.IsType<ObjectContent<object>>(response.RequestMessage.Content);
            Assert.IsType<XmlMediaTypeFormatter>(content.Formatter);
        }

        [Fact]
        public void PutAsync_Uri_WhenClientIsNull_ThrowsException()
        {
            HttpClient client = null;

            Assert.ThrowsArgumentNull(() => client.PutAsync(new Uri("http://www.example.com"), new object(), new JsonMediaTypeFormatter(), "text/json"), "client");
        }

        [Fact]
        public void PutAsync_Uri_WhenRequestUriIsNull_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.PutAsync((Uri)null, new object(), new JsonMediaTypeFormatter(), "text/json"),
                "An invalid request URI was provided. The request URI must either be an absolute URI or BaseAddress must be set.");
        }

        [Fact]
        public void PutAsync_Uri_WhenRequestUriIsSet_CreatesRequestWithAppropriateUri()
        {
            _client.BaseAddress = new Uri("http://example.com/");

            var result = _client.PutAsync(new Uri("myapi/", UriKind.Relative), new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Equal("http://example.com/myapi/", request.RequestUri.ToString());
        }

        [Fact]
        public void PutAsync_Uri_WhenAuthoritativeMediaTypeIsSet_CreatesRequestWithAppropriateContentType()
        {
            var result = _client.PutAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
            Assert.Equal("utf-16", request.Content.Headers.ContentType.CharSet);
        }

        [Fact]
        public void PutAsync_Uri_WhenFormatterIsSet_CreatesRequestWithObjectContentAndCorrectFormatter()
        {
            var result = _client.PutAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, _mediaTypeHeader, CancellationToken.None);

            var request = result.Result.RequestMessage;
            var content = Assert.IsType<ObjectContent<object>>(request.Content);
            Assert.Same(_formatter, content.Formatter);
        }

        [Fact]
        public void PutAsync_Uri_WhenAuthoritativeMediaTypeStringIsSet_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PutAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, mediaType, CancellationToken.None);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PutAsync_Uri_WhenAuthoritativeMediaTypeStringIsSetWithoutCT_CreatesRequestWithAppropriateContentType()
        {
            string mediaType = _mediaTypeHeader.MediaType;
            var result = _client.PutAsync(new Uri("http://example.com/myapi/"), new object(), _formatter, mediaType);

            var request = result.Result.RequestMessage;
            Assert.Equal("foo/bar", request.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void PutAsync_Uri_IssuesPutRequest()
        {
            var result = _client.PutAsync(new Uri("http://example.com/myapi/"), new object(), _formatter);

            var request = result.Result.RequestMessage;
            Assert.Same(HttpMethod.Put, request.Method);
        }

        [Fact]
        public void PutAsync_Uri_WhenMediaTypeFormatterIsNull_ThrowsException()
        {
            Assert.ThrowsArgumentNull(() => _client.PutAsync(new Uri("http://example.com"), new object(), formatter: null), "formatter");
        }
    }
}
