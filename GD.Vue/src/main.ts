import pinia from './store/index'
import App from './App.vue'
import router from './router'

// svg图标
import '@/assets/iconfont/iconfont.js' //iconfont
import 'virtual:svg-icons-register'
import SvgIcon from '@/components/SvgIcon/index.vue'
import elementIcons from '@/components/SvgIcon/svgicon'

import '@/assets/styles/index.scss' // global css

const app = createApp(App)

// 全局组件挂载
app.component('svg-icon', SvgIcon)

app.use(pinia)
app.use(router)
app.use(elementIcons)
app.mount('#app')