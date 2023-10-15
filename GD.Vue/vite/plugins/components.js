import Components from 'unplugin-vue-components/vite'
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers' // 自动导入 Element Plus 的 Api

export default function createComponents() {
    return Components({
        resolvers: [ElementPlusResolver()],
        dts: "src/components.d.ts" // 生成 `auto-import.d.ts` 全局声明
    })
}