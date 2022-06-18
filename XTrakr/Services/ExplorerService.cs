using System.Collections.Generic;
using System.IO;
using System.Linq;

using XTrakr.Interfaces;

namespace XTrakr.Services;

public class ExplorerService : IExplorerService
{
    public IEnumerable<FileInfo> GetFiles(string path) => Directory.GetFiles(path, "*.*").Select(x => new FileInfo(x));
    public IEnumerable<DirectoryInfo> GetDirectories(string path) => Directory.GetDirectories(path, "*.*").Select(x => new DirectoryInfo(x));
    public IEnumerable<DriveInfo> GetDrives() => Directory.GetLogicalDrives().Select(x => new DriveInfo(x));
}
