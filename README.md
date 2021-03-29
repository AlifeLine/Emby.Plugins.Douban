# Emby.Plugins.Douban
# Emby豆瓣削刮器
本项目是[jellyfin-plugin-douban](https://github.com/Libitum/jellyfin-plugin-douban) 的Emby版本
使用方法一致
因为豆瓣会对请求的referer 进行检查，自己对emby的开发也不熟练，故请求的部分图片会走cloudflare worker做反向代理，如果有大神的话希望帮忙改改那部分的代码。
安装方法为
- [点击这里下载最新的插件文件](https://github.com/AlifeLine/Emby.Plugins.Douban/releases)，解压出里面的 **Emby.Plugins.Douban.dll** 文件，通过ssh等方式拷贝到 Emby 的插件目录
- 常见的插件目录如下：
  - 群晖
    - /volume1/Emby/plugins
    - /var/packages/EmbyServer/var/plugins
    - /volume1/@appdata/EmbyServer/plugins
  - Windows
    - emby\programdata\plugins
- 需要**重启Emby服务**，插件才生效。
