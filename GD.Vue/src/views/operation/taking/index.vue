<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="作业单号" prop="jobCode">
                <el-input v-model="queryParams.jobCode" placeholder="请输入作业单号" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="创建时间">
                <el-date-picker v-model="dateRange" style="width: 300px" type="daterange" range-separator="-"
                    start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <!-- 工具栏 -->
        <el-row :gutter="10">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="false" :data="takingList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业状态" prop="jobType">
                <template #default="scope">
                    <el-tag type="" effect="dark"
                        v-if="scope.row.jobStatus === 0 || scope.row.adjustStatus === 0">待作业</el-tag>
                    <el-tag color="#626aef" effect="dark" v-else>已完成</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="所在仓库" prop="warehouseName" />
            <el-table-column label="所在库位" prop="locationCode" />
            <el-table-column label="账面数量" prop="bookQty" />
            <el-table-column label="盘点数量" prop="countedQty" />
            <el-table-column label="差异数量" prop="differenceQty" />
            <el-table-column label="操作人" prop="handler" />
            <el-table-column label="操作时间" prop="handlerTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="check" title="确认盘点" :disabled="scope.row.jobStatus === 0 ? false : true"
                            @click.stop="handleTaking(scope.row)" />
                        <el-button text icon="finished" title="确认调整" :disabled="scope.row.adjustStatus === 0 ? false : true"
                            @click.stop="handleAdjust(scope.row)" />
                        <el-button text icon="delete" title="删除" :disabled="scope.row.jobStatus === 0 ? false : true"
                            @click.stop="handleDelete(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 新增对话框 -->
        <el-dialog :title="title" v-model="open" width="25%" :show-close="false">
            <div class="dialogHeader">
                <div class="dialogHeader-Button">
                    <el-tooltip effect="dark" content="选择库存" placement="bottom-end">
                        <el-button circle icon="house" @click="handleSelcelStock" />
                    </el-tooltip>
                    <el-tooltip effect="dark" content="选择商品" placement="bottom-end">
                        <el-button circle icon="messageBox" @click="handleSelcelSku" :disabled="!isLocationSelect" />
                    </el-tooltip>
                </div>
                <div class=".dialogHeader-Tip">
                    <el-tooltip effect="dark" content="当目标库存不存在时，可通手动选择商品或库位来新增记录" placement="right-start">
                        <el-icon>
                            <QuestionFilled />
                        </el-icon>
                    </el-tooltip>
                </div>
            </div>
            <el-form :model="form" :rules="rules" ref="takingRef" label-width="80px">
                <el-form-item label="商品编码" prop="spuCode">
                    <el-input v-model="form.spuCode" placeholder="请输入商品编码" :readonly="true" />
                </el-form-item>
                <el-form-item label="商品名称" prop="spuName">
                    <el-input v-model="form.spuName" placeholder="请输入商品名称" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="规格编码" prop="skuCode">
                    <el-input v-model="form.skuCode" placeholder="请输入规格编码" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="所在仓库" prop="warehouseName">
                    <el-input v-model="form.warehouseName" placeholder="请输入所在仓库" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="所在库位" prop="locationCode">
                    <el-input v-model="form.locationCode" placeholder="请输入所在库位" :readonly="true"
                        :disabled="!isLocationSelect"
                        :style="isLocationSelect ? 'pointer-events: auto;' : 'pointer-events: none;'"
                        @click.native="selectLocationClick" />
                </el-form-item>
                <el-form-item label="账面数量" prop="bookQty">
                    <el-input v-model="form.bookQty" placeholder="请输入账面数量" disabled style="pointer-events: none;" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

        <!-- 盘点对话框 -->
        <el-dialog title="库存盘点" v-model="putOpen" width="25%" :show-close="false">
            <el-form :model="putForm" :rules="rules" ref="putRef" label-width="80px">
                <el-form-item label="账面数量" prop="bookQty">
                    <el-input v-model="putForm.bookQty" placeholder="请输入账面数量" disabled style="pointer-events: none;" />
                </el-form-item>
                <el-form-item label="盘点数量" prop="countedQty">
                    <el-input v-model="putForm.countedQty" placeholder="请输入盘点数量" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitPutForm">提交</el-button>
            </template>
        </el-dialog>

        <!-- 库存选择 -->
        <z-StockSelectDialog v-model:visible="stockSelectOpen" @dialogData="dialogStockData" />

        <!-- 商品选择 -->
        <z-SkuSelectDialog v-model:visible="skuSelectOpen" @dialogData="dialogSkuData" />

        <!-- 库位选择 -->
        <z-LocationSelectDialog v-model:visible="locationSelectOpen" @dialogData="dialogLocationData" />
    </div>
</template>

<script setup>
import { getAllInfo, getInfo, addInfo, confirmTaking, confirmAdjustment, deleteInfo, exportAllInfo } from "@/api/operation/taking";

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 仓库冻结列表
const takingList = ref([])
// 启用输入框展示库位选择对话框
const isLocationSelect = ref(true)
// 展示库存选择对话框
const stockSelectOpen = ref(false)
// 展示商品选择对话框
const skuSelectOpen = ref(false)
// 展示库位选择对话框
const locationSelectOpen = ref(false)
// 展示对话框
const open = ref(false)
// 展示盘点对话框
const putOpen = ref(false)
// 对话框标题
const title = ref('')
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        locationId: undefined,
        warehouseName: undefined,
        locationCode: undefined,
        bookQty: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    },
    rules: {
        spuCode: [{ required: true, message: '商品编码不能为空', trigger: 'blur' }],
        spuName: [{ required: true, message: '商品名称不能为空', trigger: 'blur' }],
        skuCode: [{ required: true, message: '规格编码不能为空', trigger: 'blur' }],
        warehouseName: [{ required: true, message: '所在库位不能为空', trigger: 'blur' }],
        locationCode: [{ required: true, message: '所在仓库不能为空', trigger: 'blur' }],
        countedQty: [{ required: true, message: '数量不能为空', trigger: 'blur', pattern: /^[0-9]+$/ }]
    },
    putForm: {
        takingId: undefined,
        bookQty: undefined,
        countedQty: undefined
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作`, visible: true, prop: 'operate' }
])
// 表单、搜索参数、规则
const { form, queryParams, rules, putForm } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        skuId: undefined,
        spuCode: undefined,
        spuName: undefined,
        skuCode: undefined,
        locationId: undefined,
        warehouseName: undefined,
        locationCode: undefined,
        bookQty: undefined
    }
    putForm.value = {
        takingId: undefined,
        bookQty: undefined,
        countedQty: undefined
    }
    proxy.resetForm('takingRef')
    proxy.resetForm('putRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        takingList.value = res.data.result
        total.value = res.data.totalNum
    })
}

/**
 * 搜索按钮操作
 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/**
 * 重置按钮操作
 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryRef')
    handleQuery()
}

/**
 * 新增按钮操作
 */
function handleAdd() {
    reset()
    title.value = "库存盘点"
    open.value = true
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有仓库盘点信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['takingRef'].validate((valid) => {
        if (valid && form.value.skuId !== undefined && form.value.locationId !== undefined) {
            addInfo(form.value).then(res => {
                proxy.$modal.msgSuccess('新增成功')
                open.value = false
                getList()
            })
        }
    })
}

/**
 * 确认盘点提交按钮
 */
function submitPutForm() {
    proxy.$refs['putRef'].validate((valid) => {
        if (valid && putForm.value.takingId !== undefined) {
            confirmTaking(putForm.value).then(res => {
                proxy.$modal.msgSuccess('盘点成功')
                putOpen.value = false
                getList();
            })
        }
    })
}

/**
 * 库存选择按钮操作
 */
function handleSelcelStock() {
    reset()
    stockSelectOpen.value = true
}

/**
 * 商品选择按钮操作
 */
function handleSelcelSku() {
    reset()
    skuSelectOpen.value = true
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    putOpen.value = false
    reset()
}

/**
 * 确认盘点按钮操作 
 * @param {行数据} row 
 */
function handleTaking(row) {
    putOpen.value = true

    getInfo(row.takingId).then(res => {
        putForm.value.takingId = res.data.takingId
        putForm.value.bookQty = res.data.bookQty
    })
}

/**
 * 确认调整按钮操作 
 * @param {行数据} row 
 */
function handleAdjust(row) {
    proxy.$modal
        .confirm(`是否确定调整${row.jobCode}?`)
        .then(function () {
            confirmAdjustment(row.takingId).then(res => {
                proxy.$modal.msgSuccess('调整成功')
                getList();
            })
        })
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    proxy
        .$modal
        .confirm(`是否确认删除${row.jobCode}的数据项？`)
        .then(() => {
            return deleteInfo(row.takingId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

/**
 * 库存选择框返回数据函数
 * @param {数据} val 
 */
function dialogStockData(val) {
    isLocationSelect.value = false
    for (var key in form.value) {
        if (key === "bookQty") {
            form.value[key] = val.availableQty
        } else {
            form.value[key] = val[key]
        }
    }
}

/**
 * 商品选择框返回数据函数
 * @param {数据} val 
 */
function dialogSkuData(val) {
    for (var key in form.value) {
        form.value[key] = val[key]
    }
}

/**
 * 库位选择框返回数据函数
 * @param {数据} val 
 */
function dialogLocationData(val) {
    form.value.locationId = val.locationId
    form.value.locationCode = val.locationCode
    form.value.warehouseName = val.warehouseName
    form.value.bookQty = 0
}

/**
 * 打开库位选择框函数
 * @param {数据} val 
 */
function selectLocationClick() {
    locationSelectOpen.value = true
}

getList()
</script>

<style scoped lang="scss">
.dialogHeader {
    display: flex;
    margin-bottom: 15px;
    margin-top: -10px;
    justify-content: space-between;
}

.dialogHeader-Button {}

.dialogHeader-Tip {
    display: flex;
    align-items: center;
}
</style>