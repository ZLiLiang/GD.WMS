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
                <el-button type="primary" plain @click="handleSeparate">
                    <el-icon>
                        <SvgIcon name="separate" />
                    </el-icon>
                    <span> 拆分加工 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button color="#626aef" plain @click="handleCombination">
                    <el-icon>
                        <SvgIcon name="combination" />
                    </el-icon>
                    <span> 组合加工 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="processList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业类型" prop="jobType">
                <template #default="scope">
                    <el-tag type="" effect="dark" v-if="scope.row.jobType === 0">拆分加工</el-tag>
                    <el-tag color="#626aef" effect="dark" v-else>组合加工</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="是否已调整" prop="processStatus">
                <template #default="scope">
                    <el-tag type="danger" v-if="scope.row.adjustStatus == 0">否</el-tag>
                    <el-tag type="success" v-else>是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="操作人" prop="processor" />
            <el-table-column label="操作时间" prop="processTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="170px" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="view" title="查看" @click.stop="handleDetail(scope.row)" />
                        <el-button text icon="check" title="确认加工" :disabled="scope.row.processStatus === 0 ? false : true"
                            @click.stop="handleProcess(scope.row)" />
                        <el-button text icon="finished" title="确认调整"
                            :disabled="scope.row.processStatus === 1 && scope.row.adjustStatus === 0 ? false : true"
                            @click.stop="handleAdjust(scope.row)" />
                        <el-button text icon="delete" title="删除"
                            :disabled="scope.row.processStatus === 0 && scope.row.adjustStatus === 0 ? false : true"
                            @click.stop="handleDelete(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-drawer :title="title" v-model="open" direction="btt" size="85%" :close-on-click-modal="false"
            :show-close="false">
            <el-form :model="form" ref="processRef" style="height: 100%;">
                <div class="form-container">
                    <div class="form-left">
                        <div class="toolbarTitle" v-show="!detailShow">
                            <span>来源</span>
                            <el-button type="primary" plain icon="plus" @click="handleSource" />
                        </div>
                        <div class="dataTable">
                            <el-table :data="form.sourceDetailList" highlight-current-row height="100%"
                                @cell-click="onCellClick" :row-class-name="onRowClassName"
                                :cell-class-name="onCellClassName">
                                <el-table-column label="商品编码" prop="spuCode" />
                                <el-table-column label="商品名称" prop="spuName" />
                                <el-table-column label="规格编码" prop="skuCode" />
                                <el-table-column prop="qty">
                                    <template #header>
                                        <span style="color: #f56c6c;">* </span>
                                        <Edit style="width: 1.1em; height: 1.1em; margin-right: 2px" />
                                        <span>数量</span>
                                    </template>
                                    <template #default="scope" v-if="detailShow">
                                        {{ scope.row.qty }}
                                    </template>
                                    <template #default="scope" v-else>
                                        <el-form-item :prop="`sourceDetailList.[${scope.row.index}].qty`"
                                            :rules="rules.qty">
                                            <div v-if="editSrcRow === scope.row.index && editSrcCol === scope.column.index">
                                                <el-input v-model="scope.row.qty"
                                                    @blur="onInputBlur(undefined, scope.row)" />
                                            </div>
                                            <div v-else class="else-content">
                                                {{ scope.row.qty }}
                                            </div>
                                        </el-form-item>
                                    </template>
                                </el-table-column>
                                <el-table-column label="商品单位" prop="unit" />
                                <el-table-column label="操作" width="70px" v-if="!detailShow">
                                    <template #default="scope">
                                        <el-button text icon="delete" title="删除"
                                            @click.stop="handleSourceDelete(scope.row)" />
                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>
                    </div>
                    <div class="form-right">
                        <div class="toolbarTitle" v-show="!detailShow">
                            <span>目标</span>
                            <el-button type="primary" plain icon="plus" @click="handleTarget" />
                        </div>
                        <div class="dataTable">
                            <el-table :data="form.targetDetailList" highlight-current-row :stripe="true" height="100%"
                                @cell-click="onCellClick" :row-class-name="onRowClassName"
                                :cell-class-name="onCellClassName">
                                <el-table-column label="商品编码" prop="spuCode" />
                                <el-table-column label="商品名称" prop="spuName" width="120px" />
                                <el-table-column label="规格编码" prop="skuCode" width="120px" />
                                <el-table-column prop="qty">
                                    <template #header>
                                        <span style="color: #f56c6c;">* </span>
                                        <Edit style="width: 1.1em; height: 1.1em; margin-right: 2px" />
                                        <span>数量</span>
                                    </template>
                                    <template #default="scope" v-if="detailShow">
                                        {{ scope.row.qty }}
                                    </template>
                                    <template #default="scope" v-else>
                                        <el-form-item :prop="`targetDetailList.[${scope.row.index}].qty`"
                                            :rules="rules.qty">
                                            <div v-if="editTarRow === scope.row.index && editTarCol === scope.column.index">
                                                <el-input v-model="scope.row.qty"
                                                    @blur="onInputBlur(undefined, scope.row)" />
                                            </div>
                                            <div v-else class="else-content">
                                                {{ scope.row.qty }}
                                            </div>
                                        </el-form-item>
                                    </template>
                                </el-table-column>
                                <el-table-column label="商品单位" prop="unit" />
                                <el-table-column prop="locationCode" width="120px">
                                    <template #header>
                                        <span style="color: #f56c6c;">* </span>
                                        <Edit style="width: 1.1em; height: 1.1em; margin-right: 2px" />
                                        <span>目标库位</span>
                                    </template>
                                    <template #default="scope" v-if="detailShow">
                                        {{ scope.row.locationCode }}
                                    </template>
                                    <template #default="scope" v-else>
                                        <el-form-item :prop="`targetDetailList.[${scope.row.index}].locationCode`"
                                            :rules="rules.locationCode">
                                            <!-- <el-input v-model="scope.row.locationCode" @click="onInputClick(scope.row)"
                                                :suffix-icon="Search" :readonly="true" /> -->
                                            <div v-if="editTarRow === scope.row.index && editTarCol === scope.column.index">
                                                <el-input v-model="scope.row.locationCode"
                                                    @blur="onInputBlur(undefined, scope.row)"
                                                    @click="onInputClick(scope.row)" :suffix-icon="Search"
                                                    :readonly="true" />
                                            </div>
                                            <div v-else class="else-content">
                                                {{ scope.row.locationCode }}
                                            </div>
                                        </el-form-item>
                                    </template>
                                </el-table-column>
                                <el-table-column label="操作" width="70px" v-if="!detailShow">
                                    <template #default="scope">
                                        <el-button text icon="delete" title="删除"
                                            @click.stop="handleTargetDelete(scope.row)" />
                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>
                    </div>
                </div>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm" v-show="!detailShow">提交</el-button>
            </template>
        </el-drawer>

        <!-- 库存选择 -->
        <z-StockSelectDialog v-model:visible="stockSelectSourceOpen" @dialogData="dialogSourceData" />

        <!-- 商品选择 -->
        <z-SkuSelectDialog v-model:visible="skuSelectTargetOpen" @dialogData="dialogTargetData" />

        <!-- 库位选择 -->
        <z-LocationSelectDialog v-model:visible="locationSelectTargetOpen" @dialogData="dialogTargetLocationData" />

    </div>
</template>

<script setup>
import { getAllInfo, getInfo, addInfo, deleteInfo, confirmProcess, confirmAdjustment, exportAllInfo } from '@/api/operation/process'

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const processList = ref([])
// 展示抽屉
const open = ref(false)
// 展示细节
const detailShow = ref(false)
// 展示来源对话框
const stockSelectSourceOpen = ref(false)
// 展示来源对话框
const skuSelectTargetOpen = ref(false)
// 展示库位选择对话框
const locationSelectTargetOpen = ref(false)
// 对话框标题
const title = ref('')
// 定义变量用于标记当前编辑的行和列
const editSrcRow = ref(null)
const editSrcCol = ref(null)
const editTarRow = ref(null)
const editTarCol = ref(null)
const editTarLocaRow = ref(null)
// 时间范围
const dateRange = ref([])
// 来源对象属性 isSource=0
const sourceProperty = ["stockId", "skuId", "ownerId", "locationId", "qty", "isSource", "isUpate", "spuCode", "spuName", "skuCode", "unit"]
// 目标对象属性 isSource=1
const targetProperty = ["skuId", "ownerId", "locationId", "qty", "isSource", "isUpate", "spuCode", "spuName", "skuCode", "unit", "locationCode"]
// 数据
const data = reactive({
    form: {
        // processId: undefined,
        jobCode: undefined,
        jobType: undefined,
        processStatus: undefined,
        detailList: [],
        sourceDetailList: [],
        targetDetailList: []
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    },
    rules: {
        qty: [{ required: true, message: '数量不能为空', trigger: 'blur' }],
        locationCode: [{ required: true, message: '数量不能为空', trigger: 'blur' }]
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作`, visible: true, prop: 'operate' }
])
// 表单、搜索参数、规则
const { form, queryParams, rules } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 重置操作表单
 */
function reset() {
    form.value = {
        jobCode: undefined,
        jobType: undefined,
        processStatus: undefined,
        detailList: [],
        sourceDetailList: [],
        targetDetailList: []
    }
    proxy.resetForm('processRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        processList.value = res.data.result
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
 * 拆分加工按钮操作
 */
function handleSeparate() {
    reset()
    title.value = "拆分加工"
    form.value.jobType = 0
    form.value.processStatus = 0
    open.value = true
}

/**
 * 组合加工按钮操作
 */
function handleCombination() {
    reset()
    title.value = "组合加工"
    form.value.jobType = 1
    form.value.processStatus = 0
    open.value = true
}

/**
 * 导出按钮操作 
 */
function handleExport() {
    proxy
        .$modal
        .confirm('是否确认导出所有仓库加工信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 查看按钮操作 
 * @param {行数据} row 
 */
function handleDetail(row) {
    reset()
    detailShow.value = true
    open.value = true
    let processId = row.processId
    getInfo(processId).then(res => {
        form.value.sourceDetailList = res.data.sourceDetailList
        form.value.targetDetailList = res.data.targetDetailList
    })
}

/**
 * 确认加工按钮操作 
 * @param {行数据} row 
 */
function handleProcess(row) {
    proxy.$modal
        .confirm(`是否确定加工${row.jobCode}?`)
        .then(function () {
            confirmProcess(row.processId).then(res => {
                proxy.$modal.msgSuccess('加工成功')
                getList();
            })
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
            confirmAdjustment(row.processId).then(res => {
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
            return deleteInfo(row.processId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['processRef'].validate((valid) => {
        if (valid) {
            if (form.value.sourceDetailList.length === 0 || form.value.targetDetailList.length === 0) {
                proxy.$modal.msgError('来源和目标不能为空')
                return
            }
            form.value.detailList = [...form.value.sourceDetailList, ...form.value.targetDetailList]
            addInfo(form.value).then(res => {
                proxy.$modal.msgSuccess('新增成功')
                open.value = false
                getList()
            })
        }
    })
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    detailShow.value = false
    reset()
}

/**
 * 来源按钮操作 
 */
function handleSource() {
    stockSelectSourceOpen.value = true
}

/**
 * 行的删除操作按钮
 * @param {行数据} row 
 */
function handleSourceDelete(row) {
    proxy.$modal
        .confirm('是否确认删除"' + row.spuName + '"的数据项？')
        .then(function () {
            let index = row.index
            let length = form.value.sourceDetailList.length
            for (let i = 0; i < length; i++) {
                if (form.value.sourceDetailList[i].index === index && form.value.sourceDetailList[i].isSource === 0) {
                    form.value.sourceDetailList.splice(i, 1)
                    return;
                }
            }
        })
        .then(() => {
            proxy.$modal.msgSuccess('删除成功')
        })
}

/**
 * 行的删除操作按钮
 * @param {行数据} row 
 */
function handleTargetDelete(row) {
    proxy.$modal
        .confirm('是否确认删除"' + row.spuName + '"的数据项？')
        .then(function () {
            let index = row.index
            let length = form.value.targetDetailList.length
            for (let i = 0; i < length; i++) {
                if (form.value.targetDetailList[i].index === index && form.value.targetDetailList[i].isSource === 1) {
                    form.value.targetDetailList.splice(i, 1)
                    return;
                }
            }
        })
        .then(() => {
            proxy.$modal.msgSuccess('删除成功')
        })
}

/**
 * 目标按钮操作 
 */
function handleTarget() {
    skuSelectTargetOpen.value = true
}

/**
 * 单元格被双击击时会触发该事件
 * @param {行数据} row 
 * @param {列数据} column 
 * @param {单元数据} cell 
 * @param {事件} event 
 */
function onCellClick(row, column, cell, event) {
    if (row.isSource === 0) {
        editSrcRow.value = row.index;
        editSrcCol.value = column.index;
    } else {
        editTarRow.value = row.index;
        editTarCol.value = column.index;
    }
}

/**
 * 把每一行的索引放进row
 * @param {行数据，行引索} param
 */
function onRowClassName({ row, rowIndex }) {
    row.index = rowIndex;
}

/**
 * 把每一列的索引放进column
 * @param {列数据，列引索} param0 
 */
function onCellClassName({ row, column, rowIndex, columnIndex }) {
    column.index = columnIndex;
}

/**
 * 库存选择框返回数据函数
 * @param {数据} val 
 */
function dialogSourceData(val) {
    if (title.value === "拆分加工" && form.value.sourceDetailList.length > 0) {
        return
    }
    let entity = {}
    for (var key in sourceProperty) {
        let property = sourceProperty[key]
        if (property === "isSource") {
            entity[property] = 0
            continue
        }
        entity[property] = val[property]
    }
    // 过滤重复项
    for (let item of form.value.sourceDetailList) {
        if (item.stockId === entity.stockId) {
            return
        }
    }
    form.value.sourceDetailList.push(entity)
}

/**
 * 商品选择框返回数据函数
 * @param {数据} val 
 */
function dialogTargetData(val) {
    if (title.value === "组合加工" && form.value.targetDetailList.length > 0) {
        return
    }
    let entity = {}
    for (var key in targetProperty) {
        let property = targetProperty[key]
        if (property === "isSource") {
            entity[property] = 1
            continue
        }
        entity[property] = val[property]
    }
    // 过滤重复项
    for (let item of form.value.targetDetailList) {
        if (item.skuId === entity.skuId) {
            return
        }
    }
    form.value.targetDetailList.push(entity)
}

/**
 * 库位选择框返回数据函数
 * @param {数据} val 
 */
function dialogTargetLocationData(val) {
    form.value.targetDetailList[editTarLocaRow.value].locationCode = val.locationCode
    form.value.targetDetailList[editTarLocaRow.value].locationId = val.locationId
    editTarLocaRow.value = null
}

/**
 * 输入框失去焦点时触发
 * @param {焦点事件} FocusEvent 
 * @param {行数据} row 
 */
function onInputBlur(FocusEvent, row) {
    if (locationSelectTargetOpen.value == true) {
        return
    }
    if (row.isSource === 0) {
        editSrcRow.value = undefined;
        editSrcCol.value = undefined;
    } else {
        editTarRow.value = undefined;
        editTarCol.value = undefined;
    }
}

/**
 * 	当选择器的输入框获得焦点时触发
 * @param {行数据} row 
 */
function onInputClick(row) {
    locationSelectTargetOpen.value = true
    editTarLocaRow.value = row.index
}

getList()
</script>

<style lang="scss" scoped >
:deep(.el-drawer__header) {
    margin-bottom: 0;
}

:deep(.el-table .cell) {
    overflow: visible;
}

:deep(.el-table .el-form-item) {
    margin-bottom: 0;
}

:deep(.el-table .el-form-item__error) {
    z-index: 5;
}

.form-container {
    display: flex;
    height: 100%;
}

.form-left {
    width: 50%;
    margin: 5px;
    padding: 5px;
    border: 0.5px solid #acabab;
    border-radius: 10px;
}

.form-right {
    width: 50%;
    margin: 5px;
    padding: 5px;
    border: 0.5px solid #acabab;
    border-radius: 10px;
}

.toolbarTitle {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-bottom: 10px;
}

.dataTable {
    height: 90%;
    overflow: auto;
}

.else-content {
    height: 30px;
}
</style>