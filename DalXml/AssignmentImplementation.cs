using DalApi;
using DO;
using System.Xml;
using System.Xml.Linq;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            StudentCallId = (int?)a.Element("StudentCallId") ?? 0,
            TutorId= (int?)a.Element("TutorId") ?? 0,
            EntryTime= (DateTime?)a.Element("EntryTime") ?? null,
            EndTime= (DateTime?)a.Element("EndTime") ?? null,
            EndOfTreatment= Enum.TryParse(a.Element("EndOfTreatment")?.Value, out EndOfTreatment result) ? result : 0
        };
            
        
    }
    
    public void Create(Assignment item)
    {
        XElement? assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        assignmentElements.Add(copy);

    }

    public void Delete(int id)
    {
        // טוען את כל האובייקטים מ-XML
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        // מחפש את ה-XElement שמייצג את האובייקט עם ה-ID המתאים
        XElement? assignmentElement = assignmentElements
            .Elements("Assignment") // עובר על כל האלמנטים מסוג Assignment
            .FirstOrDefault(el => (int)el.Element("ID") == id); // מחפש אלמנט עם ID תואם

        if (assignmentElement == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        // מסיר את האלמנט שנמצא
        assignmentElement.Remove();

        // שומר את העדכונים ל-XML
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentElements.RemoveAll();
       
    }

    public Assignment? Read(int id)
    {
        XElement? assignmentElement=XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(x => (int?)x.Element("Id") == id);
        return assignmentElement is null ? null : getAssignment(assignmentElement);
    }
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(x => getAssignment(x)).FirstOrDefault(filter);
        
    }
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        // טוען את כל האלמנטים מסוג Assignment מה-XML
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        // ממיר את האלמנטים ל-Assignment באמצעות getAssignment
        var assignments = from item in assignmentElements.Elements("Assignment")
                          select getAssignment(item);

        // מסנן לפי הפילטר אם יש כזה
        if (filter != null)
        {
            return assignments.Where(filter);
        }

        // מחזיר את כל המשימות אם אין פילטר
        return assignments;
    }


    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new DalDoesNotExistException($"An object of type assignment with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
