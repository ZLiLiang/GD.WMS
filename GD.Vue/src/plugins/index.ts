import { App } from 'vue'
import cache from './cache'
import modal from './modal'

declare global {
    interface Array<T> {
        showColumn(elem: T): Array<T>;
    }
}

export default {
    install: (app: App) => {
        // 缓存对象
        app.config.globalProperties.$cache = cache
        // 模态框对象
        app.config.globalProperties.$modal = modal

        Array.prototype.showColumn = function <T>(column: T) {
            var item = this.find((x) => x.prop == column)
            // console.log('showColumn方法', this, column, item)
            if (item) {
                return item.visible
            }
            return true
        }
    }
}
