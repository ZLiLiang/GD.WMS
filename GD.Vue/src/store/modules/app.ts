import Cookies from 'js-cookie'
import cache from '@/plugins/cache'
import defaultSettings from '@/settings'
const useAppStore = defineStore('app', {
    state: () => ({
        sidebar: {
            opened: !!+Cookies.get('sidebarStatus')!,
            // withoutAnimation: false,
            hide: false
        },
        size: cache.local.get('size') || defaultSettings.defaultSize
    }),
    actions: {
        toggleSideBar() {
            if (this.sidebar.hide) {
                return false
            }
            this.sidebar.opened = !this.sidebar.opened
            if (this.sidebar.opened) {
                Cookies.set('sidebarStatus', '1')
            } else {
                Cookies.set('sidebarStatus', '0')
            }
        },
        setSize(size:string) {
            this.size = size
            cache.local.set('size', size)
        },
        toggleSideBarHide(status:boolean) {
            this.sidebar.hide = status
        }
    }
})

export default useAppStore
