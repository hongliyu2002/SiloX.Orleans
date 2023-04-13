const silox = silox || {};
(function ()
{
    silox.utils = silox.utils || {};

    // DOM READY /////////////////////////////////////////////////////

    silox.domReady = function (fn)
    {
        if (document.readyState === "complete" || document.readyState === "interactive")
        {
            setTimeout(fn, 1);
        }
        else
        {
            document.addEventListener("DOMContentLoaded", fn);
        }
    };

    // COOKIES ///////////////////////////////////////////////////////

    /**
     * Sets a cookie value for given key.
     * This is a simple implementation created to be used by SiloX.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} value
     * @param {string} expireDate (optional). If not specified the cookie will expire at the end of session.
     * @param {string} path (optional)
     * @param {boolean} secure (optional)
     */
    silox.utils.setCookieValue = function (key, value, expireDate, path, secure)
    {
        let cookieValue = `${encodeURIComponent(key)}=`;
        if (value)
        {
            cookieValue = `${cookieValue}${encodeURIComponent(value)}`;
        }
        if (expireDate)
        {
            cookieValue = `${cookieValue}; expires=${expireDate}`;
        }
        if (path)
        {
            cookieValue = `${cookieValue}; path=${path}`;
        }
        if (secure)
        {
            cookieValue = `${cookieValue}; secure`;
        }
        document.cookie = cookieValue;
    };

    /**
     * Gets a cookie with given key.
     * This is a simple implementation created to be used by SiloX.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @returns {string|null} Cookie value or null
     */
    silox.utils.getCookieValue = function (key)
    {
        const equalities = document.cookie.split('; ');
        for (let i = 0; i < equalities.length; i++)
        {
            if (!equalities[i])
            {
                continue;
            }
            const splitted = equalities[i].split('=');
            if (splitted.length !== 2)
            {
                continue;
            }
            if (decodeURIComponent(splitted[0]) === key)
            {
                return decodeURIComponent(splitted[1] || '');
            }
        }
        return null;
    };

    /**
     * Deletes cookie for given key.
     * This is a simple implementation created to be used by SiloX.
     * Please use a complete cookie library if you need.
     * @param {string} key
     * @param {string} path (optional)
     */
    silox.utils.deleteCookie = function (key, path)
    {
        let cookieValue = `${encodeURIComponent(key)}=`;
        cookieValue = `${cookieValue}; expires=${(new Date(new Date().getTime() - 86400000)).toUTCString()}`;
        if (path)
        {
            cookieValue = `${cookieValue}; path=${path}`;
        }
        document.cookie = cookieValue;
    };

    // DOM MANIPULATION

    silox.utils.addClassToTag = function (tagName, className)
    {
        const tags = document.getElementsByTagName(tagName);
        for (let i = 0; i < tags.length; i++)
        {
            tags[i].classList.add(className);
        }
    };

    silox.utils.removeClassFromTag = function (tagName, className)
    {
        const tags = document.getElementsByTagName(tagName);
        for (let i = 0; i < tags.length; i++)
        {
            tags[i].classList.remove(className);
        }
    };

    silox.utils.hasClassOnTag = function (tagName, className)
    {
        const tags = document.getElementsByTagName(tagName);
        if (tags.length)
        {
            return tags[0].classList.contains(className);
        }
        return false;
    };

    silox.utils.replaceLinkHrefById = function (linkId, hrefValue)
    {
        const link = document.getElementById(linkId);
        if (link && link.href !== hrefValue)
        {
            link.href = hrefValue;
        }
    };

    // FULL SCREEN /////////////////

    silox.utils.toggleFullscreen = function ()
    {
        const elem = document.documentElement;
        if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement)
        {
            if (elem.requestFullscreen)
            {
                elem.requestFullscreen()
                    .then(r => console.log("Fullscreen enabled"));
            }
            else if (elem.msRequestFullscreen)
            {
                elem.msRequestFullscreen();
            }
            else if (elem.mozRequestFullScreen)
            {
                elem.mozRequestFullScreen();
            }
            else if (elem.webkitRequestFullscreen)
            {
                elem.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
            }
        }
        else
        {
            if (document.exitFullscreen)
            {
                document.exitFullscreen()
                        .then(r => console.log("Fullscreen disabled"));
            }
            else if (document.msExitFullscreen)
            {
                document.msExitFullscreen();
            }
            else if (document.mozCancelFullScreen)
            {
                document.mozCancelFullScreen();
            }
            else if (document.webkitExitFullscreen)
            {
                document.webkitExitFullscreen();
            }
        }
    };

    silox.utils.requestFullscreen = function ()
    {
        const elem = document.documentElement;
        if (!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement)
        {
            if (elem.requestFullscreen)
            {
                elem.requestFullscreen()
                    .then(r => console.log("Fullscreen enabled"));
            }
            else if (elem.msRequestFullscreen)
            {
                elem.msRequestFullscreen();
            }
            else if (elem.mozRequestFullScreen)
            {
                elem.mozRequestFullScreen();
            }
            else if (elem.webkitRequestFullscreen)
            {
                elem.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
            }
        }
    };

    silox.utils.exitFullscreen = function ()
    {
        if (!(!document.fullscreenElement && !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement))
        {
            if (document.exitFullscreen)
            {
                document.exitFullscreen()
                        .then(r => console.log("Fullscreen disabled"));
            }
            else if (document.msExitFullscreen)
            {
                document.msExitFullscreen();
            }
            else if (document.mozCancelFullScreen)
            {
                document.mozCancelFullScreen();
            }
            else if (document.webkitExitFullscreen)
            {
                document.webkitExitFullscreen();
            }
        }
    }
})();