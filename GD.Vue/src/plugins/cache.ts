const sessionCache = {
    set(key: string, value: string) {
        if (!sessionStorage) {
            return
        }
        if (key != null && value != null) {
            sessionStorage.setItem(key, value)
        }
    },
    get(key: string) {
        if (!sessionStorage) {
            return null
        }
        if (key == null) {
            return null
        }
        return sessionStorage.getItem(key)
    },
    setJSON(key: string, jsonValue: string) {
        if (jsonValue != null) {
            this.set(key, JSON.stringify(jsonValue))
        }
    },
    getJSON(key: string) {
        const value = this.get(key)
        if (value != null) {
            return JSON.parse(value)
        }
    },
    remove(key: string) {
        sessionStorage.removeItem(key);
    }
}

const localCache = {
    set(key: string, value: string) {
        if (!localStorage) {
            return
        }
        if (key != null && value != null) {
            localStorage.setItem(key, value)
        }
    },
    get(key: string) {
        if (!localStorage) {
            return null
        }
        if (key == null) {
            return null
        }
        return localStorage.getItem(key)
    },
    setJSON(key: string, jsonValue: string) {
        if (jsonValue != null) {
            this.set(key, JSON.stringify(jsonValue))
        }
    },
    getJSON(key: string) {
        const value = this.get(key)
        if (value != null) {
            return JSON.parse(value)
        }
    },
    remove(key: string) {
        localStorage.removeItem(key);
    }
}

import Cookies from 'js-cookie'

const cookie = {
    set(key: string, data: string, expires?: number | Date | undefined) {
        if (expires === undefined) {
            Cookies.set(key, data)

        } else {
            Cookies.set(key, data, { expires: expires })
        }
    },
    remove(key: string) {
        Cookies.remove(key)
    },
    get(key: string) {
        Cookies.get(key)
    }
}
export default {
    /**
     * 会话级缓存
     */
    session: sessionCache,
    /**
     * 本地缓存
     */
    local: localCache,
    /**
     * cookie存储
     */
    cookie: cookie
}