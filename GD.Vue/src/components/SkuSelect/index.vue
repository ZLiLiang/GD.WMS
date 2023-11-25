<template>
    <el-dialog title="商品选择框" v-model="open" width="900px" :show-close="false" :close-on-click-modal="false">
        <div class="container">
            <div class="left-form">
                <el-form :model="queryParams" ref="queryRef">
                    <el-form-item label="商品名称" prop="spuName">
                        <el-input v-model="queryParams.spuName" placeholder="请输入商品名称" clearable />
                    </el-form-item>
                    <el-form-item label="规格编码" prop="skuCode">
                        <el-input v-model="queryParams.skuCode" placeholder="请输入规格编码" clearable />
                    </el-form-item>
                    <div style="text-align: center;">
                        <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                        <el-button icon="refresh" @click="resetQuery">重置</el-button>
                    </div>
                </el-form>
            </div>
            <div class="right-table">
                <!-- 表格 -->
                <el-table v-loading="loading" :data="skuSelectList" highlight-current-row @row-click="onRowClick">
                    <el-table-column width="55" align="center" label="选择">
                        <template #default="scope">
                            <el-radio :label="scope.row.skuId" v-model="selectRadio">{{ ' ' }}</el-radio>
                        </template>
                    </el-table-column>
                    <el-table-column label="商品编码" prop="spuCode" />
                    <el-table-column label="商品名称" prop="spuName" />
                    <el-table-column label="规格编码" prop="skuCode" />
                    <el-table-column label="规格名称" prop="skuName" />
                    <el-table-column label="供应商名称" prop="supplierName" />
                    <el-table-column label="品牌" prop="brand" />
                    <el-table-column label="商品单位" prop="unit" />
                </el-table>

                <!-- 分页栏 -->
                <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
                    v-model:limit="queryParams.pageSize" @pagination="getList" />
            </div>
        </div>
        <template #footer>
            <el-button text @click="cancel">取消</el-button>
            <el-button type="primary" @click="submitForm">提交</el-button>
        </template>
    </el-dialog>
</template>

<script setup>
import { getSkuSelect } from '@/api/inventory/stock'

// 总条数
const total = ref(0)
// 加载...
const loading = ref(true)
// 通知列表
const skuSelectList = ref([])
// 选中的单选
const selectRadio = ref()
// 返回的数据
const dialogData = ref()
// 当时实例
const { proxy } = getCurrentInstance()
// 查询参数
const queryParams = ref({
    pageNum: 1,
    pageSize: 10,
    spuName: undefined,
    skuCode: undefined
})

const props = defineProps({
    visible: {
        type: Boolean,
        default: false
    },
})

const emits = defineEmits(["update:visible", "dialogData"]);
const open = computed({
    get: () => props.visible,
    set: (val) => {
        emits("update:visible", val)
    }
})

watch(
    () => props.visible,
    (val) => {
        if (val) {
            proxy.resetForm('queryRef')
            handleQuery()
            dialogData.value = undefined
        }
    }
)

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = queryParams.value
    getSkuSelect(params).then(res => {
        loading.value = false
        skuSelectList.value = res.data.result
        total.value = res.data.totalNum
    })
}

/**
 * 搜索操作
 */
function handleQuery() {
    selectRadio.value = undefined
    queryParams.pageNum = 1
    getList()
}

/**
 * 重置操作
 */
function resetQuery() {
    proxy.resetForm('queryRef')
    handleQuery()
}

/**
 * 选中的行
 * @param {行} row 
 * @param {列} column 
 * @param {事件} event 
 */
function onRowClick(row, column, event) {
    selectRadio.value = row.skuId
    dialogData.value = row
}

/**
 * 关闭对话框
 */
function cancel() {
    open.value = false
}

/**
 * 提交数据
 */
function submitForm() {
    emits("dialogData", dialogData.value)
    open.value = false
}

</script>

<style lang="scss" scoped>
.container {
    display: flex;
    flex-wrap: wrap; //换行
    align-content: flex-start; //紧揍排列
    // overflow: auto;
    background-color: rgb(239, 239, 239);
    width: 100%;
    height: 350px;
}

.left-form {
    width: 25%;
    padding: 10px;
}

.right-table {
    width: 75%;
    height: 100%;
    padding: 0 0 0 5px;
    overflow: auto;
}
</style>