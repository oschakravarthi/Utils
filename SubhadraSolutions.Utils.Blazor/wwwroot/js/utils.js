function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function setAttribute(element, attributeName, attributeValue) {
    element.setAttribute(attributeName, attributeValue);
}
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function setAttribute(element, attributeName, attributeValue) {
    element.setAttribute(attributeName, attributeValue);
}

function toKMNumberFormat(value) {
    var b = 1000000000;
    var m = 1000000;
    var k = 1000;

    var val = Math.abs(value);
    if (val >= b) {
        var temp = value / b;
        return (+(Math.round(temp + 'e+2') + 'e-2')) + ' B';
    }
    if (val >= m) {
        var temp = value / m;
        return (+(Math.round(temp + 'e+2') + 'e-2')) + ' M';
    }
    if (val >= k) {
        var temp = value / k;
        return (+(Math.round(temp + 'e+2') + 'e-2')) + ' K';
    }
    return value;
}