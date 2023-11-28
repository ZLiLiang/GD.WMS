<template>
    <el-dialog title="库位选择框" v-model="open" width="900px" :show-close="false" :close-on-click-modal="false">
        <div class="container">
            <div class="left-form">
                <el-form :model="queryParams" ref="queryRef">
                    <el-form-item label="库位编码" prop="locationCode">
                        <el-input v-model="queryParams.locationCode" placeholder="请输入库位编码" clearable />
                    </el-form-item>
                    <div style="text-align: center;">
                        <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                        <el-button icon="refresh" @click="resetQuery">重置</el-button>
                    </div>
                </el-form>
            </div>
            <div class="right-table">
                <!-- 表格 -->
                <el-table v-loading="loading" :data="locationList" highlight-current-row @row-click="onRowClick" height="87%">
                    <el-table-column width="55" align="center" label="选择">
                        <template #default="scope">
                            <el-radio :label="scope.row.locationId" v-model="selectRadio">{{ ' ' }}</el-radio>
                        </template>
                    </el-table-column>
                    <el-table-column label="仓库名称" prop="warehouseName" />
                    <el-table-column label="库区名称" prop="regionName"/>
                    <el-table-column label="库区类型" prop="regionProperty" >
                        <template #default="scope">
                            <span>{{ regionPropertyOptions[scope.row.regionProperty] }}</span>
                        </template>
                    </el-table-column>
                    <el-table-column label="库位编码" prop="locationCode" />
                    <el-table-column label="库位长(m)" prop="locationLength" />
                    <el-table-column label="库位宽(m)" prop="locationWidth"  />
                    <el-table-column label="库位高(m)" prop="locationHeight"  />
                    <el-table-column label="库位容积(m³)" prop="locationVolume"  />
                    <el-table-column label="库位承重(kg)" prop="locationLoad"  />
                    <el-table-column label="巷道号" prop="roadwayNumber"  />
                    <el-table-column label="货架号" prop="shelfNumber"  />
                    <el-table-column label="层号" prop="layerNumber"  />
                    <el-table-column label="位号" prop="tagNumber"  />
                    <el-table-column label="是否有效" prop="isValid" >
                        <template #default="scope">
                            <el-tag v-if="scope.row.isValid === 1" type="success">是</el-tag>
                            <el-tag v-else type="danger">否</el-tag>
                        </template>
                    </el-table-column>
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
import { getAllInfo } from '@/api/basic/location'

// 总条数
const total = ref(0)
// 加载...
const loading = ref(true)
// 通知列表
const locationList = ref([])
// 选中的单选
const selectRadio = ref()
// 库区类型
const regionPropertyOptions = ["拣货区", "备货区", "收货区", "退货区", "次品区", "存货区"]
// 返回的数据
const dialogData = ref()
// 当时实例
const { proxy } = getCurrentInstance()
// 查询参数
const queryParams = ref({
    pageNum: 1,
    pageSize: 10,
    locationCode: undefined
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
    getAllInfo(params).then(res => {
        loading.value = false
        locationList.value = res.data.result
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
    selectRadio.value = row.locationId
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