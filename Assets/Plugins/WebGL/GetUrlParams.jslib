mergeInto(LibraryManager.library, {
    GetUrlParamJS: function (paramNamePtr) {
        var paramName = UTF8ToString(paramNamePtr);
        var urlParams = new URLSearchParams(window.location.search);
        var value = urlParams.get(paramName) || "";
        var bufferSize = lengthBytesUTF8(value) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(value, buffer, bufferSize);
        return buffer;
    }
});