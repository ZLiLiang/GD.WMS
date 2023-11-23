import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAll(query) {
    return request({
        url: '/basic/commodity',
        method: 'get',
        params: query
    })
}

/**
 * 通过id查询单个商品
 * @param {商品ID} commodityId 
 * @returns 封装请求
 */
export function getCommodityInfo(commodityId) {
    return request({
        url: '/basic/commodity/' + praseStrZero(commodityId),
        method: 'get'
    })
}

/**
 * 根据id删除商品
 * @param {商品ID} commodityId 
 * @returns 封装请求
 */
export function deleteById(commodityId) {
    return request({
        url: '/basic/commodity/' + praseStrZero(commodityId),
        method: 'delete'
    })
}

/**
 * 根据sku的Id删除商品sku信息
 * @param {sku的Id} commoditySKUId 
 * @returns 
 */
export function deleteSKUById(commoditySKUId){
    return request({
        url: '/basic/commodity/sku/' + praseStrZero(commoditySKUId),
        method: 'delete'
    })
}

/**
 * 新增商品
 * @param {商品实体} entity 
 * @returns 封装请求
 */
export function addCommodityInfo(entity) {
    return request({
        url: '/basic/commodity/add',
        method: 'post',
        data: entity
    })
}

/**
 * 根据商品id修改商品信息
 * @param {商品ID} commodityId 
 * @param {商品实体} entity 
 * @returns 封装请求
 */
export function editCommodityInfo(commodityId, entity) {
    return request({
        url: '/basic/commodity/edit/' + praseStrZero(commodityId),
        method: 'post',
        data: entity
    })
}

/**
 * 获取商品类别选项
 * @returns 封装请求
 */
export function getCategoryOptions() {
    return request({
        url: '/basic/commodity/categoryOptions',
        method: 'get'
    })
}

/**
 * 获取供应商选项
 * @returns 封装请求
 */
export function getSupplierOptions() {
    return request({
        url: '/basic/commodity//supplierOptions',
        method: 'get'
    })
}

/**
 * 导出商品信息
 */
export async function exportCommodity() {
    await downFile('/basic/commodity/export')
}