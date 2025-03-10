using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DialogueSystem {
    public class CommandManager<T> {
        private readonly Dictionary<string, Action<T>> commandMap = new();
        public CommandManager(string managerId, string[] loadAssembly) {
            // 加载当前程序集
            LoadCommandsFromAssembly(Assembly.GetExecutingAssembly(), managerId);

            // 尝试加载 Assembly-CSharp
            try {
                Assembly assemblyCSharp = Assembly.Load("Assembly-CSharp");
                if (assemblyCSharp != null) 
                    LoadCommandsFromAssembly(assemblyCSharp, managerId);
            } catch {
                // 如果不存在该程序集，则忽略或记录日志
            }

            // 加载 loadAssembly 中指定的程序集
            if (loadAssembly != null) {
                foreach (var assemblyName in loadAssembly) {
                    try {
                        Assembly asm = Assembly.Load(assemblyName);
                        if (asm != null) {
                            LoadCommandsFromAssembly(asm, managerId);
                        }
                    } catch {
                        Debug.Log($"Failed to load assembly {assemblyName}!");
                    }
                }
            }
        }
        
        private void LoadCommandsFromAssembly(Assembly assembly, string managerId) {
            foreach (Type type in assembly.GetTypes()) {
                // 获取类型中所有方法（公有、非公有，静态与实例方法）
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                    // 获取所有标记了 CommandAttribute 的特性
                    var attributes = method.GetCustomAttributes(typeof(CommandAttribute), false);
                    foreach (CommandAttribute attr in attributes) {
                        // 仅处理 ManagerId 匹配的方法
                        if (attr.ManagerId != managerId)
                            continue;

                        // 检查方法参数：必须只有一个，并且类型为 T
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(T))
                            continue;

                        try {
                            Action<T> action;
                            if (method.IsStatic) {
                                // 静态方法无需实例化
                                action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), method);
                            } else {
                                // 实例方法需要先创建实例
                                object instance = Activator.CreateInstance(type);
                                action = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), instance, method);
                            }

                            // 将命令添加到字典中（如果存在相同的命令名，可根据需要选择覆盖或忽略）
                            if (!commandMap.ContainsKey(attr.CommandName))
                                commandMap.Add(attr.CommandName, action);
                            else
                                commandMap[attr.CommandName] = action;
                        } catch (Exception ex) {
                            // 处理委托创建失败的情况，比如记录错误信息
                        }
                    }
                }
            }
        }
    }
}