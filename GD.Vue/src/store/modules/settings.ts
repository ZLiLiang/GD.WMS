import defaultSettings from '@/settings'

const { sideTheme, theme, fixedHeader, sidebarLogo, title } = defaultSettings

const useSettingsStore = defineStore('settings', {
    state: () => ({
        title: title,
        theme: theme,
        sideTheme: sideTheme,
        fixedHeader: fixedHeader,
        sidebarLogo: sidebarLogo
    }),
})

export default useSettingsStore
