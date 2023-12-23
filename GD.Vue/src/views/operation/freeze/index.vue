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
                <el-button type="primary" plain icon="lock" @click="handleAdd">冻结</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="success" plain icon="unlock" @click="handleAdd">解冻</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="false" :data="freezeList" highlight-current-row>
            <el-table-column label="作业单号" prop="jobCode" />
            <el-table-column label="作业状态" prop="jobType">
                <template #default="scope">
                    <el-tag type="" effect="dark" v-if="scope.row.jobType === 0">冻结</el-tag>
                    <el-tag color="#626aef" effect="dark" v-else>解冻</el-tag>
                </template>
            </el-table-column>
            <el-table-column label="所在仓库" prop="warehouseName" />
            <el-table-column label="所在库位" prop="locationCode" />
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="操作人" prop="handler" />
            <el-table-column label="操作时间" prop="handlerTime" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />

            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="view" title="查看" @click.stop="handleUpdate(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 对话框 -->
        <el-dialog :title="title" v-model="open" width="600px" :show-close="false" :draggable="true">
            <el-form :model="form" :rules="rules" ref="supplierRef" label-width="80px">

            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

    </div>
</template>

<script setup>
// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 仓库冻结列表
const freezeList = ref([])
// 展示对话框
const open = ref(false)
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
        locationCode: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        jobCode: undefined
    },
    rules: {}
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

    }
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
 * 新增按钮操作
 */
function handleAdd() {

}

/**
 * 导出按钮操作 
 */
function handleExport() {

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
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleUpdate(row) {

}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {

}

getList()
</script>

<style></style>