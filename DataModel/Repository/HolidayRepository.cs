namespace DataModel.Repository;

using Microsoft.EntityFrameworkCore;

using DataModel.Model;
using DataModel.Mapper;

using Domain.Model;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class HolidayRepository : GenericRepository<Holiday>, IHolidayRepository
{    
    HolidayMapper _holidayMapper;
    ColaboratorsIdMapper _colaboratorsIdMapper;
    public HolidayRepository(AbsanteeContext context, HolidayMapper mapper, ColaboratorsIdMapper colaboratorsIdMapper) : base(context!)
    {
        _holidayMapper = mapper;
        _colaboratorsIdMapper = colaboratorsIdMapper;
    }

    public async Task<Holiday> GetHolidayByIdAsync(long id)
        {
            try {
                HolidayDataModel holidayDataModel = await _context.Set<HolidayDataModel>()
                        .Include(c => c.colaboratorId)
                        .FirstAsync(c => c.Id==id);

                Holiday holiday = _holidayMapper.ToDomain(holidayDataModel);

                return holiday;
            }
            catch
            {
                throw;
            }
        }


    public async Task<IEnumerable<Holiday>> getHolidaysByColabIdInPeriod(long colabId, DateOnly startDate, DateOnly endDate)
    {
        try {
            IEnumerable<HolidayDataModel> holidaysDataModel = await _context.Set<HolidayDataModel>()
                    .Include(c => c.colaboratorId)
                    .Where(c => c.colaboratorId.Id==colabId && c._endDate>startDate && c._startDate<endDate)
                    .ToListAsync();

            if(holidaysDataModel== null){
                return null;
            }

            IEnumerable<Holiday> holidays = _holidayMapper.ToDomain(holidaysDataModel);

            return holidays;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Holiday> AddHoliday(Holiday holiday)
    {
        try {

            ColaboratorsIdDataModel colaboratorDataModel = await _context.Set<ColaboratorsIdDataModel>()
                        .FirstAsync(c => c.Id == holiday.GetColaborator());
            HolidayDataModel holidayDataModel = _holidayMapper.ToDataModel(holiday, colaboratorDataModel);

            EntityEntry<HolidayDataModel> holidayDataModelEntityEntry = _context.Set<HolidayDataModel>().Add(holidayDataModel);
            
            await _context.SaveChangesAsync();

            HolidayDataModel holidayDataModelSaved = holidayDataModelEntityEntry.Entity;

            Holiday holidaySaved = _holidayMapper.ToDomain(holidayDataModelSaved);

            return holidaySaved;    
        }
        catch
        {
            throw;
        }
    }

    
    public async Task<bool> HolidayExists(long id)
    {
    Console.WriteLine($"Checking if holiday with ID {id} exists");
    var exists = await _context.Set<HolidayDataModel>().AnyAsync(e => e.Id == id);
    Console.WriteLine($"Exists: {exists}");
    return exists;
    }
}