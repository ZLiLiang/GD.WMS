import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/receive/asn'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAllInfo(query) {
    return request({
        url: baseUrl,
        method: 'get',
        params: query
    })
}

/**
 * 确认卸货
 * @param {行数据Id} id 
 */
export function unload(id) {
    let url = `${baseUrl}/unload/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 取消到货
 * @param {行数据Id} id 
 */
export function cancelArrive(id) {
    let url = `${baseUrl}/cancelArrive/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 导出待卸货信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/unloadExport`
    await downFile(url)
}