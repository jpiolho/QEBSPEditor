using QEBSPEditor.Core.EntParsing;
using QEBSPEditor.Models.BSPFiles;
using System.Text;

namespace QEBSPEditor.Models;

public class BSPEntitySourceEditor
{
    private IBSPFileEntities _bsp;
    private List<Entity> _entities;


    public IBSPFileEntities BSP => _bsp;
    public string Source { get => _bsp.Entities; }
    public IReadOnlyList<Entity> Entities => _entities;

    public BSPEntitySourceEditor(IBSPFileEntities bsp)
    {
        _bsp = bsp;
        _entities = new List<Entity>();
    }

    public void ParseEntitiesFromSource()
    {
        _entities = QuickEntParser.ParseEntities(_bsp.Entities);
    }

    public void SetSource(string source, bool parseEntities)
    {
        _bsp.Entities = source;

        if (parseEntities)
            ParseEntitiesFromSource();
    }

    public Entity AddEntity(string classname)
    {
        var entity = new Entity();
        entity.SourceHint = new EntitySourceHint()
        {
            OffsetStart = _bsp.Entities.Length,
            OffsetEnd = _bsp.Entities.Length,
        };

        entity.SetKeyValue("classname", classname);

        // Add it to the source
        UpdateEntity(entity);

        _entities.Add(entity);
        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);

        // Remove it from the source code
        if (entity.SourceHint != null)
        {
            var sh = entity.SourceHint;
            var trim = GetEndOfLineTrim(sh.OffsetEnd);
            _bsp.Entities = _bsp.Entities.Substring(0, sh.OffsetStart) + _bsp.Entities.Substring(sh.OffsetEnd + trim);

            // Decrease the sourcehint for all other entities after
            var length = sh.Length + trim;
            foreach (var ent in _entities)
            {
                if (ent != entity && ent.SourceHint is not null && ent.SourceHint.OffsetStart > sh.OffsetEnd)
                {
                    ent.SourceHint.OffsetStart -= length;
                    ent.SourceHint.OffsetEnd -= length;
                }
            }
        }
    }

    private int GetEndOfLineTrim(int index)
    {
        int count = 0;
        while (index < _bsp.Entities.Length)
        {
            if (char.IsWhiteSpace(_bsp.Entities[index++]))
                count++;
            else
                break;
        }

        return count;
    }

    public void UpdateEntity(Entity entity)
    {
        // There's only anything to update if there's source
        if (entity.SourceHint == null)
            return;

        var sb = new StringBuilder();

        sb.AppendLine("{");
        foreach (var kv in entity.KeyValues)
        {
            var key = kv.Key.Replace("\"", "\\\"");
            var value = kv.Value.Replace("\"", "\\\"");

            sb.AppendLine($"  \"{key}\" \"{value}\"");
        }
        sb.AppendLine("}");


        _bsp.Entities = _bsp.Entities.Substring(0, entity.SourceHint.OffsetStart) + sb.ToString() + _bsp.Entities.Substring(entity.SourceHint.OffsetEnd);

        // Update the end of the source hint
        var oldSourceHintLength = entity.SourceHint.Length;
        entity.SourceHint.OffsetEnd = entity.SourceHint.OffsetStart + (sb.ToString().TrimEnd().Length);
        var lengthDiff = entity.SourceHint.Length - oldSourceHintLength;

        // Update the sourcehint for all other entities after this one
        foreach (var ent in _entities.Where(e => e != entity && e.SourceHint is not null && e.SourceHint.OffsetStart > entity.SourceHint.OffsetStart))
        {
            if (ent != entity &&
                ent.SourceHint is not null &&
                ent.SourceHint.OffsetStart > entity.SourceHint.OffsetStart
            )
            {
                ent.SourceHint.OffsetStart += lengthDiff;
                ent.SourceHint.OffsetEnd += lengthDiff;
            }
        }
    }


    public int GetEntityIndex(Entity entity) => _entities.IndexOf(entity);
}
