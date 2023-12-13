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
                <el-button type="primary" plain icon="plus" @click="handleSeparate">
                    <el-icon>
                        <SvgIcon name="separate" />
                    </el-icon>
                    <span> 拆分加工 </span>
                </el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="success" plain icon="plus" @click="handleCombination">
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
        <el-table v-loading="false" :data="processList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业类型" prop="jobType">
                <template #default="scope">
                    <span v-if="scope.row.jobCode === 0">拆分加工</span>
                    <span v-else>组合加工</span>
                </template>
            </el-table-column>
            <el-table-column label="是否已调整" prop="processStatus">
                <template #default="scope">
                    <el-tag type="danger" v-if="scope.row.processStatus == 0">否</el-tag>
                    <el-tag type="success" v-else>是</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="操作人" prop="processor" />
            <el-table-column label="操作时间" prop="processTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="查看" @click.stop="handleDetail(scope.row)" />
                        <el-button text icon="edit" title="确认加工" @click.stop="handleProcess(scope.row)" />
                        <el-button text icon="edit" title="确认调整" @click.stop="handleAdjust(scope.row)" />
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-drawer :title="title" v-model="open" direction="btt" size="85%" :close-on-click-modal="false">
            <el-form :model="form" :rules="rules" ref="processRef" style="height: 100%;">
                <div class="form-container">
                    <div class="form-left">
                        <div class="toolbarTitle">
                            <span>来源</span>
                            <el-button type="primary" plain icon="plus" @click="handleSource" />
                        </div>
                        <div class="dataTable">
                            <el-table :data="form.detailList.filter((val) => val.isSource === 0)" highlight-current-row
                                :stripe="true" height="100%" @cell-click="onCellClick" :row-class-name="onRowClassName"
                                :cell-class-name="onCellClassName">
                                <el-table-column label="商品编码" prop="spuCode" />
                                <el-table-column label="商品名称" prop="spuName" />
                                <el-table-column label="规格编码" prop="skuCode" />
                                <el-table-column label="数量" prop="qty">
                                    <template #default="scope">
                                        <el-form-item :prop="`detailList[${scope.row.index}].qty`" :rules="rules.qty">
                                            <div v-if="editRow === scope.row.index && editCol === scope.column.index">
                                                <el-input v-model="scope.row.qty" @blur="onInputBlur" />
                                            </div>
                                            <div v-else class="else-content">
                                                {{ scope.row.qty }}
                                            </div>
                                        </el-form-item>
                                    </template>
                                </el-table-column>
                                <el-table-column label="商品单位" prop="unit" />
                                <el-table-column label="操作">
                                    <template #default="scope">

                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>
                    </div>
                    <div class="form-right">
                        <div class="toolbarTitle">
                            <span>目标</span>
                            <el-button type="primary" plain icon="plus" @click="handleTarget" />
                        </div>
                        <div class="dataTable">
                            <el-table :data="form.detailList.filter((val) => val.isSource === 1)" highlight-current-row
                                :stripe="true" height="100%">
                                <el-table-column label="商品编码" prop="spuCode" />
                                <el-table-column label="商品名称" prop="spuName" />
                                <el-table-column label="规格编码" prop="skuCode" />
                                <el-table-column label="数量" prop="qty">
                                    <template #default="scope">
                                        <el-form-item>

                                        </el-form-item>
                                    </template>
                                </el-table-column>
                                <el-table-column label="商品单位" prop="unit" />
                                <el-table-column label="目标库位" prop="locationCode">
                                    <template #default="scope">

                                    </template>
                                </el-table-column>
                                <el-table-column label="操作">
                                    <template #default="scope">

                                    </template>
                                </el-table-column>
                            </el-table>
                        </div>
                    </div>
                </div>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-drawer>

        <z-StockSelectDialog v-model:visible="stockSelectSourceOpen" @dialogData="dialogSourceData" />
    </div>
</template>

<script setup>

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
// 展示来源对话框
const stockSelectSourceOpen = ref(false)
// 对话框标题
const title = ref('')
// 定义变量用于标记当前编辑的行和列
const editRow = ref(null)
const editCol = ref(null)
// 时间范围
const dateRange = ref([])
// 来源对象属性 isSource=0
const sourceProperty = ["skuId", "ownerId", "locationId", "qty", "isSource", "isUpate", "spuCode", "spuName", "skuCode", "unit"]
// 目标对象属性 isSource=1
const targetProperty = ["skuId", "ownerId", "locationId", "qty", "isSource", "isUpate", "spuCode", "spuName", "skuCode", "unit", "locationCode"]
// 数据
const data = reactive({
    form: {
        // processId: undefined,
        jobCode: undefined,
        jobType: undefined,
        processStatus: undefined,
        detailList: []
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
    },
    rules: {
        qty: [{ required: true, message: '数量不能为空', trigger: 'blur' }],
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
        detailList: []
    }
    proxy.resetForm('processRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
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
    title.value = "拆分加工"
    open.value = true
}

/**
 * 组合加工按钮操作
 */
function handleCombination() {

}

/**
 * 导出按钮操作 
 */
function handleExport() {

}

/**
 * 查看按钮操作 
 * @param {行数据} row 
 */
function handleDetail(row) {

}

/**
 * 确认加工按钮操作 
 * @param {行数据} row 
 */
function handleProcess(row) {

}

/**
 * 确认调整按钮操作 
 * @param {行数据} row 
 */
function handleAdjust(row) {

}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {

}

/**
 * 提交按钮
 */
function submitForm() {
    proxy.$refs['supplierRef'].validate((valid) => {

    })
}

/**
 * 取消按钮
 */
function cancel() {
    open.value = false
    reset()
}

/**
 * 来源按钮操作 
 */
function handleSource() {
    stockSelectSourceOpen.value = true
}

/**
 * 目标按钮操作 
 */
function handleTarget() {

}

/**
 * 单元格被双击击时会触发该事件
 * @param {行数据} row 
 * @param {列数据} column 
 * @param {单元数据} cell 
 * @param {事件} event 
 */
function onCellClick(row, column, cell, event) {
    editRow.value = row.index;
    editCol.value = column.index;
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
 * 商品选择框返回数据函数
 * @param {数据} val 
 */
function dialogSourceData(val) {
    let entity = {}
    for (var key in sourceProperty) {
        let property = sourceProperty[key]
        if (property === "isSource") {
            entity[property] = 0
            continue
        }
        entity[property] = val[property]
    }
    form.value.detailList.push(entity)
}

/**
 * 输入框失去焦点时触发
 * @param {焦点事件} FocusEvent 
 */
function onInputBlur(FocusEvent) {
    editRow.value = undefined;
    editCol.value = undefined;
}

getList()
</script>

<style lang="scss" scoped >
:deep(.el-drawer__header) {
    margin-bottom: 0;
}

:deep(.el-form-item) {
    margin-top: 18px;
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