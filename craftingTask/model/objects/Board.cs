using craftingTask.persistence.managers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace craftingTask.model.objects
{
  public class Board : INotifyPropertyChanged
  {
    private string _name = string.Empty;
    private string _color = string.Empty;
    private DateTime _modificationDate;

    public long BoardId { get; set; }

    public string Name
    {
      get => _name;
      set
      {
        if (_name != value)
        {
          _name = value;
          OnPropertyChanged(nameof(Name));
        }
      }
    }

    public string Color
    {
      get => _color;
      set
      {
        if (_color != value)
        {
          _color = value;
          OnPropertyChanged(nameof(Color));
        }
      }
    }

    public DateTime CreationDate { get; set; }

    public DateTime ModificationDate
    {
      get => _modificationDate;
      set
      {
        if (_modificationDate != value)
        {
          _modificationDate = value;
          OnPropertyChanged(nameof(ModificationDate));
        }
      }
    }

    private List<Board> boardList { get; set; }
    private long LastBoardId { get; set; }
    private BoardManager boardManager { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public Board()
    {
      boardManager = new BoardManager();
      boardList = new List<Board>();

      LastBoardId = boardManager.GetBoardLastId();
      this.BoardId = LastBoardId;
    }

    public Board(string inputName, string inputColor)
    {
      boardManager = new BoardManager();
      boardList = new List<Board>();

      LastBoardId = boardManager.GetBoardLastId();
      this.BoardId = LastBoardId;
      this.Name = inputName;
      this.Color = inputColor;
      this.CreationDate = DateTime.UtcNow;
      this.ModificationDate = DateTime.UtcNow;
    }

    public Board(string inputName, string inputColor, DateTime inputModificationDate)
    {
      boardManager = new BoardManager();
      boardList = new List<Board>();

      LastBoardId = boardManager.GetBoardLastId();
      this.BoardId = LastBoardId;
      this.Name = inputName;
      this.Color = inputColor;
      this.ModificationDate = inputModificationDate;
      this.CreationDate = DateTime.UtcNow;
    }

    public Board(long inputBoardId, string inputName, string inputColor, DateTime inputCreationDate, DateTime inputModificationDate)
    {
      boardManager = new BoardManager();
      boardList = new List<Board>();

      this.BoardId = inputBoardId;
      this.Name = inputName;
      this.Color = inputColor;
      this.ModificationDate = inputModificationDate;
      this.CreationDate = inputCreationDate;
    }

    public void Add()
    {
      boardManager = new BoardManager();
      boardManager.AddBoard(this);
    }
    public void Update()
    {
      boardManager = new BoardManager();
      boardManager.UpdateBoard(this);
    }
    public void Delete()
    {
      boardManager = new BoardManager();
      boardManager.DeleteBoard(this);
    }
  }
}
