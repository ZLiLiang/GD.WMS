import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

// 查询商品类别
export function getAll(query) {
    return request({
        url: '/warehousemanagement/category',
        method: 'get',
        params: query
    })
}

export function getAllTree(){
    return request({
        url: '/warehousemanagement/category/tree',
        method: 'get'
    })
}

// 根据id查询
export function getCategoryInfo(categoryId) {
    return request({
        url: '/warehousemanagement/category/' + praseStrZero(categoryId),
        method: 'get'
    })
}

// 根据id删除
export function deleteById(categoryId) {
    return request({
        url: '/warehousemanagement/category/' + praseStrZero(categoryId),
        method: 'delete'
    })
}

// 新增公司信息
export function addCategoryInfo(entity) {
    return request({
        url: '/warehousemanagement/category/add',
        method: 'post',
        params: entity
    })
}

// 编辑公司信息
export function editCategoryInfo(categoryId, entity) {
    return request({
        url: '/warehousemanagement/category/edit/' + praseStrZero(categoryId),
        method: 'post',
        params: entity
    })
}

// 导出公司
export async function exportCategory() {
    await downFile('/warehousemanagement/category/export')
}