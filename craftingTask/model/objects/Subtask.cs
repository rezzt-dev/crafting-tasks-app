using System;

namespace craftingTask.model.objects
{
    public class Subtask
    {
        public long SubtaskId { get; set; }
        public long ParentTaskId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }

        // Constructor vacío
        public Subtask() { }

        // Constructor para crear nuevas subtareas
        public Subtask(long parentTaskId, string title, int order)
        {
            ParentTaskId = parentTaskId;
            Title = title;
            IsCompleted = false;
            Order = order;
        }
    }
}
