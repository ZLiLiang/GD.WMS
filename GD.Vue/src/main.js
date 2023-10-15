import { createApp } from 'vue'

import '@/assets/styles/index.scss' // global css
// 注册指令
import plugins from './plugins' // plugins
import App from '@/App.vue'
import router from '@/router/index'
import pinia from '@/store/index'

// svg图标
import '@/assets/iconfont/iconfont' //iconfont
import 'virtual:svg-icons-register'
import SvgIcon from '@/components/SvgIcon/index.vue'
import elementIcons from '@/components/SvgIcon/svgicon'

const app = createApp(App)

// 全局组件挂载
app.component('svg-icon', SvgIcon)

app.use(pinia)
app.use(router)
app.use(elementIcons)
app.use(plugins)
app.mount('#app')