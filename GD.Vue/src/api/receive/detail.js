import request from '@/utils/request'
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
 * 导出细明信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/detailExport`
    await downFile(url)
}