using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace linq_slideviews;

public class ParsingTask
{
    /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
    /// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
    /// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
    public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
    {
        return lines
            .Select(line =>
                {
                    var splitedText = line.Split(new[] { ';' }, 3, StringSplitOptions.RemoveEmptyEntries);
                    if (splitedText.Length < 3
                    || !int.TryParse(splitedText[0], out int slideId))
                    {
                        return null;
                    }

                    switch (splitedText[1])
                    {
                        case "theory":
                            return new SlideRecord(slideId, SlideType.Theory, splitedText[2]);
                        case "quiz":
                            return new SlideRecord(slideId, SlideType.Quiz, splitedText[2]);
                        case "exercise":
                            return new SlideRecord(slideId, SlideType.Exercise, splitedText[2]);
                        default:
                            return null;
                    }
                })
                .Where(x => x != null)
            .ToDictionary(slideRecord => slideRecord.SlideId);
    }

    /// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
    /// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
    /// Такой словарь можно получить методом ParseSlideRecords</param>
    /// <returns>Список информации о посещениях</returns>
    /// <exception cref="FormatException">Если среди строк есть некорректные</exception>
    public static IEnumerable<VisitRecord> ParseVisitRecords(
        IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
    {
        if (lines == null)
        {
            throw new InvalidProgramException();
        }
        string format = "yyyy-MM-dd;HH:mm:ss";

        return lines
            .Skip(1).Select(x =>
            {
                var splitedText = x.Split(new[] { ';' }, 3, StringSplitOptions.RemoveEmptyEntries);

                if (splitedText.Length < 3 ||
                    !int.TryParse(splitedText[0], out int userId) ||
                    !int.TryParse(splitedText[1], out int slideId) ||
                    !slides.TryGetValue(slideId, out SlideRecord record) ||
                    !DateTime.TryParseExact(splitedText[2], format, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime date))
                {
                    throw new FormatException($"Wrong line [{x}]");
                }

                return new VisitRecord(userId, slideId, date, record.SlideType);
            })
            .Where(x => x != null);
    }
}