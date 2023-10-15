import autoImport from 'unplugin-auto-import/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers' // 自动导入 Element Plus 的 Api

export default function createAutoImport() {
    return autoImport({
        imports: [
            'vue',
            'vue-router',
            'pinia'
        ],
        resolvers: [ElementPlusResolver()],
        dts: "src/auto-import.d.ts" // 生成 `auto-import.d.ts` 全局声明
    })
}