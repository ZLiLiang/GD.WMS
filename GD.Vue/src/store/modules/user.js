
const useUserStore = defineStore('user', {
    state: () => ({
        userInfo: {
            welcomeMessage: 'Welcome',
            nickName: '管理员',
            welcomeContent: '晚上好',
            avatar: ''
        },
        name: '管理员',
        avatar: '',
    })
})
export default useUserStore