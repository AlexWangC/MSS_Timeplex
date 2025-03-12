using System;

namespace DialogueSystem {
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class CommandAttribute : Attribute {
        public string ManagerId { get; }
        public string CommandName { get; }
        public CommandAttribute(string managerId, string commandName) {
            ManagerId = managerId;
            CommandName = commandName;
        }
    }
}