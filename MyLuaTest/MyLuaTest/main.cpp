#include <lua.hpp>
#include <windows.h> 
int main() {

	lua_State*l = luaL_newstate();
	luaL_openlibs(l);
	luaL_dofile(l, "main.lua");
	lua_close(l);
	system("pause"); //Ϊ�˱�֤��windows��Դ�������д�main.exe�����ˡ���Ҫwindows.h��

}
