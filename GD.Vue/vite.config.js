import { defineConfig } from 'vite'
import path from 'path'
import createVitePlugins from './vite/index'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: createVitePlugins(),
  resolve:{
    // https://cn.vitejs.dev/config/#resolve-alias
    alias: {
      // 设置路径
      '~': path.resolve(__dirname, './'),
      // 设置别名
      '@': path.resolve(__dirname, './src')
    },
    // 导入时想要省略的扩展名列表
    // https://cn.vitejs.dev/config/#resolve-extensions
    extensions: ['.mjs', '.js', '.ts', '.jsx', '.tsx', '.json', '.vue']
  }
})
