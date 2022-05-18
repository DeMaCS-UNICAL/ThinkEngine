
xcopy /f "./bin/Debug/Editor/System.Runtime.CompilerServices.Unsafe.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Editor/System.Numerics.Vectors.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Editor/System.Memory.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Editor/System.Collections.Immutable.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Editor/System.Buffers.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Editor/ThinkEngine.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug//Standalone/ThinkEngine.dll" "./ThinkEngine/Assets/Plugins/" /Y
xcopy /f "./bin/Debug/Editor/Antlr4.Runtime.Standard.dll" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/dlv2.exe" "./ThinkEngine/StreamingAssets/ThinkEngine/lib/" /Y
xcopy /f "./bin/Debug/Editor/ThinkEngine.dll.meta" "./ThinkEngine/Assets/Plugins/ThinkEngineDLL/" /Y
xcopy /f "./bin/Debug/Standalone/ThinkEngine.dll.meta" "./ThinkEngine/Assets/Plugins/" /Y
PowerShell compress-archive -force .\ThinkEngine\* .\ThinkEnginePlugin.zip