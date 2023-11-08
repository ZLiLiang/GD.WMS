<template>
    <div class="app-container">
        <!-- 搜索表单 -->
        <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch">
            <el-form-item label="上级商品类别" prop="parentId">
                <el-cascader class="w100" :options="categoryQueryOptions" style="width: 160px"
                    :props="{ checkStrictly: true, value: 'categoryId', label: 'categoryName', emitPath: false }"
                    placeholder="请选择上级菜单" clearable v-model="queryParams.parentId">
                    <template #default="{ node, data }">
                        <span>{{ data.categoryName }}</span>
                        <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
                    </template>
                </el-cascader>
            </el-form-item>
            <el-form-item label="商品类别" prop="categoryName">
                <el-input v-model="queryParams.categoryName" placeholder="请输入商品类别" clearable style="width: 155px" />
            </el-form-item>
            <el-form-item label="创建人" prop="createBy">
                <el-input v-model="queryParams.creator" placeholder="请输入创建人" clearable style="width: 155px" />
            </el-form-item>
            <el-form-item label="创建时间">
                <el-date-picker v-model="dateRange" style="width: 175px" type="daterange" range-separator="-"
                    start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <!-- 工具按钮 -->
        <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="info" plain icon="Sort" @click="toggleExpandAll">展开/折叠</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <!-- 表格 -->
        <el-table v-if="refreshTable" v-loading="loading" :data="categoryList" highlight-current-row row-key="categoryId"
            border :default-expand-all="expandAll">
            <el-table-column label="商品类别" prop="categoryName" sortable />
            <el-table-column label="创建人" prop="createBy" align="center" sortable />
            <el-table-column label="创建时间" prop="createTime" align="center" sortable />
            <el-table-column label="操作" align="center">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
                        </el-button>
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
                        </el-button>
                    </div>
                </template>
            </el-table-column>
        </el-table>

        <!-- 弹框 -->
        <el-dialog :title="title" v-model="open" width="420px" :show-close="false">
            <el-form ref="categoryRef" :model="form" :rules="rules" label-width="100px">
                <el-form-item label="上级商品类别">
                    <el-cascader class="w100" :options="categoryOptions"
                        :props="{ checkStrictly: true, value: 'categoryId', label: 'categoryName', emitPath: false }"
                        placeholder="请选择上级菜单" clearable v-model="form.parentId">
                        <template #default="{ node, data }">
                            <span>{{ data.categoryName }}</span>
                            <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
                        </template>
                    </el-cascader>
                </el-form-item>
                <el-form-item label="商品类别" prop="categoryName">
                    <el-input v-model="form.categoryName" placeholder="请输入商品类别" />
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
import { getAll, getCategoryInfo, addCategoryInfo, editCategoryInfo, deleteById, exportCategory, getAllTree } from "@/api/warehousManagement/category";

// 总条数
const total = ref(0)
const showSearch = ref(false)
const categoryOptions = ref([])
const categoryQueryOptions = ref([])
const loading = ref(false)
const categoryList = ref([])
const open = ref(false)
const title = ref('')
const expandAll = ref(false)
const refreshTable = ref(true)
const dateRange = ref([])
const data = reactive({
    form: {
        categoryId: undefined,
        categoryName: undefined,
        parentId: undefined
    },
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        categoryName: undefined,
        creator: undefined,
        parentId: undefined
    },
    rules: {
        categoryName: [{ required: true, message: '商品类别不能为空', trigger: 'blur' }],
    }
})

const { form, queryParams, rules } = toRefs(data)
const { proxy } = getCurrentInstance()

/** 重置操作表单 */
function reset() {
    form.value = {
        categoryId: undefined,
        categoryName: undefined,
        parentId: undefined
    }
    proxy.resetForm('categoryRef')
}

/** 查询商品类别列表 */
function getList() {
    loading.value = true
    getAll(proxy.addDateRange(queryParams.value, dateRange.value)).then(res => {
        loading.value = false
        categoryList.value = res.data.result
        total.value = res.data.totalNum
    })
    getAllTree().then(res => {
        categoryQueryOptions.value  = res.data
    })
}

getList()

function getTreeselect() {
    getAllTree().then(res => {
        categoryOptions.value = res.data
    })
}

/** 新增按钮操作 */
function handleAdd() {
    reset()
    getTreeselect()
    open.value = true
    title.value = "新增"
}

/** 导出按钮操作 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有商品类别数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportCategory()
        })
}

/** 展开/折叠操作 */
function toggleExpandAll() {
    refreshTable.value = false;
    expandAll.value = !expandAll.value
    nextTick(() => {
        refreshTable.value = true;
    })
}

/** 修改按钮操作 ok */
function handleUpdate(row) {
    reset()
    getTreeselect()
    const cateogryId = row.categoryId
    getCategoryInfo(cateogryId).then((response) => {
        form.value = response.data
        open.value = true
        title.value = '修改角色'
    })
    open.value = true
}

/** 删除按钮操作 */
function handleDelete(row) {
    const cateogryId = row.categoryId
    proxy.$modal
        .confirm('是否确认删除用户编号为"' + row.categoryName + '"的数据项？')
        .then(function () {
            return deleteById(cateogryId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

/** 提交按钮 */
function submitForm() {
    proxy.$refs['categoryRef'].validate((valid) => {
        if (valid) {
            if (form.value.categoryId != undefined) {
                console.log(form.value);
                editCategoryInfo(form.value.categoryId, form.value).then(res => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                addCategoryInfo(form.value).then(res => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    getList()
                })
            }
        }
    })
}

/** 取消按钮 */
function cancel() {
    open.value = false
    reset()
}

/** 搜索按钮操作 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/** 重置按钮操作 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryRef')
    handleQuery()
}

</script>

<style></style>