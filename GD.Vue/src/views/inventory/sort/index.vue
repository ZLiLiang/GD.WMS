<template>
    <div class="app-container">
        <!-- 搜索功能 -->
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="供应商名称" prop="supplierName">
                <el-input v-model="queryParams.supplierName" placeholder="请输入供应商名称" clearable style="width: 160px" />
            </el-form-item>
            <el-form-item label="规格名称" prop="skuName">
                <el-input v-model="queryParams.skuName" placeholder="请输入规格名称" clearable style="width: 160px" />
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
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-loading="loading" :data="sortList" highlight-current-row>
            <el-table-column label="到货通知书编号" prop="asnNo" />
            <el-table-column label="商品编码" prop="spuCode" />
            <el-table-column label="商品名称" prop="spuName" />
            <el-table-column label="规格编码" prop="skuCode" />
            <el-table-column label="规格名称" prop="skuName" />
            <el-table-column label="货主名称" prop="ownerName" />
            <el-table-column label="供应商名称" prop="supplierName" />
            <el-table-column label="到货通知书数据" prop="asnQty" />
            <el-table-column label="总重量" prop="weight" />
            <el-table-column label="总体积" prop="volume" />
            <el-table-column label="分拣数量" prop="sortedQty" />
            <el-table-column label="创建人" prop="createBy" v-if="columns.showColumn('createBy')" />
            <el-table-column label="创建时间" prop="createTime" v-if="columns.showColumn('createTime')" />
            <el-table-column label="操作人" prop="updateBy" v-if="columns.showColumn('updateBy')" />
            <el-table-column label="操作时间" prop="updateTime" v-if="columns.showColumn('updateTime')" />
            <!-- 操作行 -->
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleEdit(scope.row)" />
                        <el-button text icon="check" title="确认" @click.stop="handleUpdate(scope.row)" />
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)" />
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 分页栏 -->
        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <el-dialog v-model="open" title="编辑分拣数量" :show-close="false" width="300px">
            <el-form :model="form" :rules="rules" ref="asnNoticeRef">
                <el-form-item label="分拣数量" prop="sortedQty">
                    <el-input v-model="form.sortedQty" placeholder="请输入分拣数量" clearable />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>
    </div>
</template>

<script setup>
import { getAllInfo, getInfo, editInfo, sorted, cancelUnload, exportAllInfo } from '@/api/receive/sort'

// 总条数
const total = ref(0)
// 展示搜索界面
const showSearch = ref(false)
// 加载...
const loading = ref(true)
// 供应商列表
const sortList = ref([])
// 展示对话框
const open = ref(false)
// 时间范围
const dateRange = ref([])
// 数据
const data = reactive({
    form: {},
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        supplierName: undefined,
        skuName: undefined,
        asnStatus: 2
    },
    rules: {
        sortedQty: [{ required: true, message: '分拣数量不为空且只能为数字', trigger: 'blur', pattern: /^[0-9]+$/ }]
    }
})
// 列显隐信息
const columns = ref([
    { key: 0, label: `创建人`, visible: false, prop: 'createBy' },
    { key: 1, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 2, label: `操作人`, visible: false, prop: 'updateBy' },
    { key: 3, label: `操作时间`, visible: false, prop: 'updateTime' },
    { key: 4, label: `操作`, visible: true, prop: 'operate' }
])
// 搜索参数
const { form, queryParams, rules } = toRefs(data)
// 当时实例
const { proxy } = getCurrentInstance()

/**
 * 重置操作表单
 */
function reset() {
    form.value = {}
    proxy.resetForm('asnNoticeRef')
}

/**
 * 获取列表
 */
function getList() {
    loading.value = true
    let params = proxy.addDateRange(queryParams.value, dateRange.value)
    getAllInfo(params).then(res => {
        loading.value = false
        sortList.value = res.data.result
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
 * 导出按钮操作 
 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有待分拣数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportAllInfo()
        })
}

/**
 * 修改按钮操作 
 * @param {行数据} row 
 */
function handleEdit(row) {
    open.value = true
    getInfo(row.asnId).then((res) => {
        form.value = res.data
    })
}

/**
 * 确定按钮操作 
 * @param {行数据} row 
 */
function handleUpdate(row) {
    proxy.$modal
        .confirm('是否确认分拣?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(() => {
            sorted(row.asnId).then(() => {
                proxy.$modal.msgSuccess("确认成功！")
                getList()
            })
        })
}

/**
 * 删除按钮操作
 * @param {行数据} row 
 */
function handleDelete(row) {
    proxy.$modal
        .confirm('是否取消卸货?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(() => {
            cancelUnload(row.asnId).then(() => {
                proxy.$modal.msgSuccess("确认成功！")
                getList()
            })
        })
}

/**
 * 对话框取消按钮
 */
function cancel() {
    reset()
    open.value = false
}

/**
 * 对话框提交表单按钮
 */
function submitForm() {
    proxy.$refs['asnNoticeRef'].validate((valid) => {
        if (valid && form.value.asnId != undefined) {
            editInfo(form.value.asnId, form.value).then(res => {
                proxy.$modal.msgSuccess('修改成功')
                open.value = false
                getList()
            })
        }
    })
}

getList()
</script>

<style></style>