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
                    var cookie = $.trim(cookies[i]);
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
    getPrivate: function (url, data, callback, async) {
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
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    getPublic: function (url, data, callback, async) {
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
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPrivate: function (url, data, callback, async) {
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
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    postPublic: function (url, data, callback, async) {
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
            success: function (data) {
                if (typeof (call) == 'function') {
                    try { call(data); }
                    catch (e) { console.error(e.message); }
                }
            }
        });
    },
    datagridConfig: {
        loadMsg: '',
        pageSize: 20,
        pageList: [15, 20, 30, 50, 100],
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
            var pagedata = { total: 0, rows: [] };
            if (data.Data) {
                pagedata.total = data.Data.TotalCount;
                pagedata.rows = data.Data.Data;
            }
            return pagedata;
        },
        onLoadSuccess: function (data) {
            $(this).datagrid('resize');
        },
    }
});

