$.extend({
    /**  
     1. 设置cookie的值，把name变量的值设为value    
    example $.cookie(’name’, ‘value’); 
     2.新建一个cookie 包括有效期 路径 域名等 
    example $.cookie(’name’, ‘value’, {expires: 7, path: ‘/’, domain: ‘jquery.com’, secure: true}); 
    3.新建cookie 
    example $.cookie(’name’, ‘value’); 
    4.删除一个cookie 
    example $.cookie(’name’, null); 
    5.取一个cookie(name)值给myvar 
    var account= $.cookie('name'); 
    **/
    cookie: function (name, value, options) {
        if (typeof value != 'undefined') { // name and value given, set cookie 
            options = options || {};
            if (value === null) {
                value = '';
                options.expires = -1;
            }
            var expires = '';
            if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                var date;
                if (typeof options.expires == 'number') {
                    date = new Date();
                    date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                } else {
                    date = options.expires;
                }
                expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE 
            }
            var path = options.path ? '; path=' + options.path : '';
            var domain = options.domain ? '; domain=' + options.domain : '';
            var secure = options.secure ? '; secure' : '';
            document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
        } else { // only name given, get cookie 
            var cookieValue = null;
            if (document.cookie && document.cookie != '') {
                var cookies = document.cookie.split(';');
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = jQuery.trim(cookies[i]);
                    // Does this cookie string begin with the name we want? 
                    if (cookie.substring(0, name.length + 1) == (name + '=')) {
                        cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                        break;
                    }
                }
            }
            return cookieValue;
        }
    }
});

$.extend({
    MsgStatus: {
        OK: 0,
        Error: 100,
        ServerError: 101,
        NeedLogin: 200,
        NeedAuth: 201,
        NeedLicence: 300
    },
    stringToHex: function (str) {
        var val = "";
        try{
            for (var i = 0; i < str.length; i++) {
                var s = str.charCodeAt(i).toString(16);
                while (s.length < 4) s = '0' + s;
                val = val + s;
            }
        }
        catch(ex){}

        return val;
    },
    hexToString: function (str) {
        var val = "";
        try {
            if (str.length >= 4 && str.length % 4 == 0) {
                var i = 0;
                while (i < str.length) {
                    var s = str.substring(i, i + 4);
                    var j = 0;
                    while (s.length > 1 && s.substring(0, 1) == '0') s = s.substring(1);
                    var c = parseInt(s, 16);
                    val = val + String.fromCharCode(c);
                    i += 4;
                }
            }
        }
        catch (ex) { }

        return val;
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    account: {
        validator: function (value, param) {
            var reg = /^[a-zA-Z]+$/;
            var reg2 = /^[a-zA-Z]+[a-zA-Z0-9]+$/;
            var reg3 = /^[a-zA-Z]+[_\.\-]+[a-zA-Z0-9]+$/;
            var reg4 = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            return reg.test(value) || reg2.test(value) || reg3.test(value) || reg4.test(value);
        },
        message: '账号必须由字母、数字、下划线、小数点、@符<br/>号组成，并以字母开头，字母或数字结尾，或者<br/>是Email格式！'
    },
    compare: {
        validator: function (value, param) {
            var s = param[0];
            if (s.indexOf('#') < 0) s = '#' + s;
            return value == $(s).val();
        },
        message: '{1}'
    },
    noCompare: {
        validator: function (value, param) {
            var s = param[0];
            if (s.indexOf('#') < 0) s = '#' + s;
            return value != $(s).val();
        },
        message: '{1}'
    },
    func: {
        validator: function (value, param) {
            var call = param[0];
            var a = false;
            try { call(value); }
            catch (ex) { }
            return a;
        },
        message: '{1}'
    }
});

$.extend({
    getUrlParam: function (name, url) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        url = url || window.location.search;
        var i = url.indexOf('?');
        if (i >= 0) {
            var r = url.substring(i).match(reg);
            if (r) {
                return unescape(r[2]);
            }
        }

        return '';
    },
    validMsg: function (data, is) {
        switch (data.Status) {
            case $.ModelStatus.NeedLogin:
                if (is) {
                    $.messager.alert('提示', data.Message || '未登录或登录已超时！', 'error', function () {
                        window.location.href = '/index.html';
                    });
                }
                break;
            case $.ModelStatus.Error:
            case $.ModelStatus.ServerError:
            case $.ModelStatus.NeedAuth:
            case $.ModelStatus.NeedLicence:
            case $.ModelStatus.NeedEnabled:
            case $.ModelStatus.NotExist:
                if (is) {
                    if (data.Message) {
                        $.messager.alert('提示', data.Message, 'error');
                    }
                    break;
                }
            case $.ModelStatus.OK:
                return true;
            default:
                break;
        }
        return false;
    },
    findArrayObject: function (arr, name, value) {
        if ($.isArray(arr) && name) {
            for (var i = 0; i < arr.length; i++) {
                var m = arr[i];
                if (m[name] == value) {
                    return m;
                }
            }
        }
        return null;
    },
    existArrayObject: function (arr, value) {
        if ($.isArray(arr)) {
            for (var i = 0; i < arr.length; i++) {
                var m = arr[i];
                if (m == value) {
                    return true;
                }
            }
        }
        return false;
    },
    copyArray: function (arr) {
        var result = [];
        if ($.isArray(arr)) {
            for (var i = 0; i < arr.length; i++) {
                var m = arr[i];
                if (typeof(m) == 'object') {
                    var _m = {};
                    for (var name in m) {
                        _m[name] = m[name];
                    }
                    result.push(_m);
                }
                else {
                    result.push(m);
                }
            }
        }

        return result;
    },
    getPrivate: function (url, data, callback) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        $.ajax({
            type: 'GET',
            url: url,
            global: false,
            async: true,
            cache: false,
            data: d,
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    getPublic: function (url, data, callback) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        $.ajax({
            type: 'GET',
            url: url,
            global: true,
            async: true,
            cache: false,
            data: d,
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPrivate: function (url, data, callback) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        $.ajax({
            type: 'post',
            url: url,
            global: false,
            async: true,
            cache: false,
            data: d,
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPublic: function (url, data, callback) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        $.ajax({
            type: 'post',
            url: url,
            global: true,
            async: true,
            cache: false,
            data: d,
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    }
});