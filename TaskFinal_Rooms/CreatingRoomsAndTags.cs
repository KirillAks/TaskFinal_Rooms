using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFinal_Rooms
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreatingRoomsAndTags : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction transaction = new Transaction(doc, "Создание помещений");
            transaction.Start();
            foreach (var level in levels)
            {
                doc.Create.NewRooms2(level);
            }

            transaction.Commit();
            
            List<Room> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .OfType<Room>()
                .ToList();
            
            Transaction transaction2 = new Transaction(doc, "Создание марок");
            transaction2.Start();
                        
            foreach (var room in rooms)
            {
                string levelName = room.Level.Name.ToString();                
                room.Name = $"{levelName}_{room.Number}";
                UV roomTagLocation = new UV(0, 0);                
                doc.Create.NewRoomTag(new LinkElementId(room.Id), roomTagLocation, null);
            }

            transaction2.Commit();

            
            return Result.Succeeded;
        }
    }
}
