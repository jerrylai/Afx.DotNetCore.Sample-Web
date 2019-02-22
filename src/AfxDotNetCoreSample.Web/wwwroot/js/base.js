
(function (factory) {
 /*!
 * jQuery Cookie Plugin v1.4.1
 * https://github.com/carhartl/jquery-cookie
 *
 * Copyright 2006, 2014 Klaus Hartl
 * Released under the MIT license
 */
    if (typeof define === 'function' && define.amd) {
        // AMD (Register as an anonymous module)
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(jQuery);
    }
}(function ($) {

    var pluses = /\+/g;

    function encode(s) {
        return config.raw ? s : encodeURIComponent(s);
    }

    function decode(s) {
        return config.raw ? s : decodeURIComponent(s);
    }

    function stringifyCookieValue(value) {
        return encode(config.json ? JSON.stringify(value) : String(value));
    }

    function parseCookieValue(s) {
        if (s.indexOf('"') === 0) {
            // This is a quoted cookie as according to RFC2068, unescape...
            s = s.slice(1, -1).replace(/\\"/g, '"').replace(/\\\\/g, '\\');
        }

        try {
            // Replace server-side written pluses with spaces.
            // If we can't decode the cookie, ignore it, it's unusable.
            // If we can't parse the cookie, ignore it, it's unusable.
            s = decodeURIComponent(s.replace(pluses, ' '));
            return config.json ? JSON.parse(s) : s;
        } catch (e) { }
    }

    function read(s, converter) {
        var value = config.raw ? s : parseCookieValue(s);
        return $.isFunction(converter) ? converter(value) : value;
    }

    var config = $.cookie = function (key, value, options) {

        // Write

        if (arguments.length > 1 && !$.isFunction(value)) {
            options = $.extend({}, config.defaults, options);

            if (typeof options.expires === 'number') {
                var days = options.expires, t = options.expires = new Date();
                t.setMilliseconds(t.getMilliseconds() + days * 864e+5);
            }

            return (document.cookie = [
                encode(key), '=', stringifyCookieValue(value),
                options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
                options.path ? '; path=' + options.path : '',
                options.domain ? '; domain=' + options.domain : '',
                options.secure ? '; secure' : ''
            ].join(''));
        }

        // Read

        var result = key ? undefined : {},
            // To prevent the for loop in the first place assign an empty array
            // in case there are no cookies at all. Also prevents odd result when
            // calling $.cookie().
            cookies = document.cookie ? document.cookie.split('; ') : [],
            i = 0,
            l = cookies.length;

        for (; i < l; i++) {
            var parts = cookies[i].split('='),
                name = decode(parts.shift()),
                cookie = parts.join('=');

            if (key === name) {
                // If second argument (value) is a function it's a converter...
                result = read(cookie, value);
                break;
            }

            // Prevent storing a cookie that we couldn't decode.
            if (!key && (cookie = read(cookie)) !== undefined) {
                result[name] = cookie;
            }
        }

        return result;
    };

    config.defaults = {};

    $.removeCookie = function (key, options) {
        // Must not alter options, thus extending a fresh object...
        $.cookie(key, '', $.extend({}, options, { expires: -1 }));
        return !$.cookie(key);
    };
}));

Date.prototype.Format = function (fmt) { //author: meizz   
    var d = new Date();
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
} 

$.extend({
    stringToHex: function (str) {
        var val = "";
        try {
            for (var i = 0; i < str.length; i++) {
                var s = str.charCodeAt(i).toString(16);
                while (s.length < 4) s = '0' + s;
                val = val + s;
            }
        }
        catch (ex) { }

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
    },
    getUrlObj: function (url) {
        var obj = {};
        url = url || window.location.search;
        var i = url.indexOf('?');
        if (i >= 0) {
            url = url.substring(i + 1);
            var arr = url.split('&');
            for (var i = 0; i < arr.length; i++) {
                var av = arr[i].split('=');
                obj[av[0]] = av.length < 2 ? '' : av[1];
            }
        }

        return obj;
    },
    getUrlParam: function (name, url) {
        var obj = $.getUrlObj(url);
        for (var n in obj) {
            if (n == name) {
                return obj[n];
            }
        }
        return '';
    },
    setUrlParam: function (obj, url) {
        url = url || window.location.href;
        var s = url;
        var i = url.indexOf('?');
        if (i > 0) s = url.substring(0, i);
        if (obj) {
            var p = '';
            for (var n in obj) {
                var v = obj[n];
                if (v) p = p.concat('&', n, '=', v);
            }
            if (p.length > 0) {
                p = p.substring(1);
                s = s.concat('?', p);
            }
        }
        return s;
    },
    htmlEncode: function (html) {
        var temp = document.createElement("div");
        if (temp.textContent != null) {
            temp.textContent = html;
        } else {
            temp.innerText = html;
        }
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    getFormData: function (form) {
        if (typeof (form) == 'string') {
            if (form.indexOf('#') != 0) form = '#' + form;
        }
        var arr = $(form).serializeArray();
        var data = {};
        $.each(arr, function () {
            data[this.name] = this.value;
        });

        return data;
    },
    findArray: function (arr, name, value) {
        if ($.isArray(arr) && name) {
            for (var i = 0; i < arr.length; i++) {
                var m = arr[i];
                if (m[name] == value) {
                    return { index: i, data: m };
                }
            }
        }
        return null;
    },
    queryArray: function (arr, name, value) {
        var list = [];
        if ($.isArray(arr) && name) {
            for (var i = 0; i < arr.length; i++) {
                var m = arr[i];
                if (m[name] == value) {
                    list.push({ index: i, data: m });
                }
            }
        }
        return list;
    },
    existArray: function (arr, name, value) {
        if ($.isArray(arr)) {
            if (typeof (value) == 'undefined') {
                for (var i = 0; i < arr.length; i++) {
                    var m = arr[i];
                    if (m == value) {
                        return true;
                    }
                }
            }
            else {
                for (var i = 0; i < arr.length; i++) {
                    var m = arr[i];
                    if (m[name] == value) {
                        return true;
                    }
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
                if (typeof (m) == 'object') {
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
    numFormat: function (value, n, m) {
        if (typeof (value) != 'number') value = parseFloat(value);
        if (typeof (n) != 'number') n = parseInt(n);
        var s = value.toFixed(n).toString();
        var arr = s.split('.');
        if (arr.length == 2) {
            var ps = arr[1];
            while (ps.length > 0 && ps.endsWith('0')) {
                ps = ps.substring(0, ps.length - 1);
            }
            if (ps.length == 0) arr.splice(1, 1);
            else arr[1] = ps;
        }

        if (typeof (m) == "number" && m > 0 && arr.length == 2) {
            var i = m - arr[0].length;
            if (i == 0) {
                arr.splice(1, 1);
            }
            else if (i > 0 && arr[1].length > i) {
                arr[1] = arr[1].substring(0, i);
            }
        }

        arr[0] = arr[0].replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        s = arr.join('.');
        return s;
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    loginAccount: {
        validator: function (value, param) {
            var reg = /^[a-zA-Z]+$/;
            var reg2 = /^[a-zA-Z]+[a-zA-Z0-9]+$/;
            var reg3 = /^[a-zA-Z]+[_\.\-]+[a-zA-Z0-9]+$/;
            var reg4 = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            var mobilereg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
            return reg.test(value) || reg2.test(value) || reg3.test(value) || reg4.test(value) || mobilereg.test(value);
        },
        message: '登录账号格式不正确！'
    },
    account: {
        validator: function (value, param) {
            var reg = /^[a-zA-Z]+$/;
            var reg2 = /^[a-zA-Z]+[a-zA-Z0-9]+$/;
            var reg3 = /^[a-zA-Z]+[_\.\-]+[a-zA-Z0-9]+$/;
            return reg.test(value) || reg2.test(value) || reg3.test(value);
        },
        message: '账号必须由字母、数字、下划线、小数点、减号<br/>组成，并以字母开头，字母或数字结尾！'
    },
    mail: {
        validator: function (value, param) {
            var reg = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            return reg.test(value);
        },
        message: 'Email格式不正确！'
    },
    mobile: {
        validator: function (value, param) {
            var reg = /^[1][3,4,5,7,8,9][0-9]{9}$/;
            return reg.test(value);
        },
        message: '手机号码格式不正确！'
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
            try { a = call(value); }
            catch (ex) { }
            return a;
        },
        message: '{1}'
    },
});

$.extend({
    showDataMsg: function (msg, data) {
        var showmsg = msg || '请求失败！';
        if (data && data.Msg) {
            showmsg = data.Msg;
        }
        $.messager.alert('提示', showmsg, 'error');
    },
    getPrivate: function (url, data, callback, async, state) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        var isasync = true;
        if (typeof (async) == 'boolean') isasync = async;
        $.ajax({
            type: 'GET',
            url: url,
            global: false,
            async: isasync,
            cache: false,
            data: d,
            success: function (dd) {
                if (typeof (call) == 'function') {
                    try { if (state) call(dd, state); else call(dd); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    getPublic: function (url, data, callback, async, state) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        var isasync = true;
        if (typeof (async) == 'boolean') isasync = async;
        $.ajax({
            type: 'GET',
            url: url,
            global: true,
            async: isasync,
            cache: false,
            data: d,
            success: function (dd) {
                if (typeof (call) == 'function') {
                    try { if (state) call(dd, state); else call(dd); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPrivate: function (url, data, callback, async, state) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        var isasync = true;
        if (typeof (async) == 'boolean') isasync = async;
        $.ajax({
            type: 'post',
            url: url,
            global: false,
            async: isasync,
            cache: false,
            data: d,
            success: function (dd) {
                if (typeof (call) == 'function') {
                    try { if (state) call(dd, state); else call(dd); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPublic: function (url, data, callback, async, state) {
        var d = data, call = callback;
        if (typeof (d) == 'function') {
            d = {};
            call = data;
        }
        var isasync = true;
        if (typeof (async) == 'boolean') isasync = async;
        $.ajax({
            type: 'post',
            url: url,
            global: true,
            async: isasync,
            cache: false,
            data: d,
            success: function (dd) {
                if (typeof (call) == 'function') {
                    try { if (state) call(dd, state); else call(dd); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    datagridConfig: {
        loadMsg: '',
        pageSize: 25,
        pageList: [10, 15, 20, 25, 30, 35, 40, 50, 100],
        onBeforeLoad: function (param) {
            param.PageSize = param.rows;
            param.PageIndex = param.page;
            delete param.rows;
            delete param.page;
            if (param.sort && param.order) {
                var orderarr = param.sort.split(',');
                var sortarr = param.order.split(',');
                if (orderarr.length == sortarr.length) {
                    var orderby = '';
                    for (var i = 0; i < orderarr.length; i++) {
                        var s1 = orderarr[i].trim();
                        var s2 = sortarr[i].trim();
                        if (s1 && s2) orderby = orderby.concat(s1, ' ', s2, ',')
                    }
                    if (orderby.length > 0) orderby = orderby.substring(0, orderby.length - 1);
                    param.Orderby = orderby;
                }
                delete param.sort;
                delete param.order;
            }
        },
        loadFilter: function (data) {
            if ($.isArray(data) || $.isArray(data.rows)) return data;

            var pagedata = { total: 0, rows: [] };
            if (data.Data) {
                pagedata.total = data.Data.TotalCount;
                if (data.Data.Data) pagedata.rows = data.Data.Data;
                if (data.Data.TotalData) {
                    pagedata.footer = data.Data.TotalData;
                }
            }
            
            return pagedata;
        },
        onLoadSuccess: function (data) {
            $(this).datagrid('resize');
        },
    }
});

