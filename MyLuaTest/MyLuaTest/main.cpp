#include <lua.hpp>
#include <windows.h> 
int main() {

	lua_State*l = luaL_newstate();
	luaL_openlibs(l);
	luaL_dofile(l, "main.lua");
	lua_close(l);
	system("pause"); //为了保证在windows资源管理器中打开main.exe不闪退。需要windows.h。

}
