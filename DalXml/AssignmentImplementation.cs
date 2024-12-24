using DalApi;
using DO;
using System.Xml;
using System.Xml.Linq;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// Converts an XElement to an Assignment object.
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            StudentCallId = (int?)a.Element("StudentCallId") ?? 0,
            TutorId = (int?)a.Element("TutorId") ?? 0,
            EntryTime = (DateTime?)a.Element("EntryTime") ?? null,
            EndTime = (DateTime?)a.Element("EndTime") ?? null,
            EndOfTreatment = Enum.TryParse(a.Element("EndOfTreatment")?.Value, out EndOfTreatment result) ? result : 0
        };
    }

    /// Converts an Assignment object to an XElement.
    static XElement GetXElement(Assignment assignment)
    {
        var elements = new List<XElement>();

        if (assignment.Id != 0)
            elements.Add(new XElement("Id", assignment.Id));

        if (assignment.StudentCallId != 0)
            elements.Add(new XElement("StudentCallId", assignment.StudentCallId));

        if (assignment.TutorId != 0)
            elements.Add(new XElement("TutorId", assignment.TutorId));

        if (assignment.EntryTime != null)
            elements.Add(new XElement("EntryTime", assignment.EntryTime));

        if (assignment.EndTime != null)
            elements.Add(new XElement("EndTime", assignment.EndTime));

        if (assignment.EndOfTreatment != null)
            elements.Add(new XElement("EndOfTreatment", assignment.EndOfTreatment.ToString()));

        return new XElement("Assignment", elements);
    }

    /// Creates a new Assignment and saves it to the XML file.
    public void Create(Assignment item)
    {
        XElement? assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        int id = Config.NextAssignmentId; // Generates a new ID.
        Assignment copy = item with { Id = id }; // Creates a copy of the object with the new ID.

        assignmentElements.Add(GetXElement(copy));
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);
    }

    /// Deletes an Assignment by its ID.
    public void Delete(int id)
    {
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        XElement? assignmentElement = assignmentElements
            .Elements("Assignment")
            .FirstOrDefault(el => (int)el.Element("Id") == id);

        if (assignmentElement == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        assignmentElement.Remove();
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);
    }

    /// Deletes all Assignments from the XML file.
    public void DeleteAll()
    {
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        assignmentElements.RemoveAll();
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);
    }

    /// Reads an Assignment by its ID.
    public Assignment? Read(int id)
    {
        XElement? assignmentElement = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements()
            .FirstOrDefault(x => (int?)x.Element("Id") == id);
        return assignmentElement is null ? null : getAssignment(assignmentElement);
    }

    /// Reads an Assignment based on a filter condition.
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements()
            .Select(x => getAssignment(x))
            .FirstOrDefault(filter);
    }

    /// Reads all Assignments with an optional filter.
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        var assignments = from item in assignmentElements.Elements("Assignment")
                          select getAssignment(item);

        if (filter != null)
        {
            return assignments.Where(filter);
        }

        return assignments;
    }

    /// Updates an existing Assignment.
    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new DalDoesNotExistException($"An object of type assignment with such an {item.Id} does not exist");

        Delete(item.Id); // Deletes the old assignment.

        XElement? assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        Assignment copy = item with { Id = item.Id }; // Creates a copy of the object with the new ID.

        assignmentElements.Add(GetXElement(copy));
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);

    }
}
