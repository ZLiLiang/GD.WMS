import vue from '@vitejs/plugin-vue'

import createAutoImport from './plugins/auto-import'
import createComponents from './plugins/components'
import createSvgIcon from './plugins/svg-icon'

export default function createVitePlugins(){
    const vitePlugins = [vue()]
    vitePlugins.push(createAutoImport())
    vitePlugins.push(createComponents())
    vitePlugins.push(createSvgIcon())

    return vitePlugins
}