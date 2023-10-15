import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const store = createPinia().use(piniaPluginPersistedstate)
store.use(piniaPluginPersistedstate)
export default store
