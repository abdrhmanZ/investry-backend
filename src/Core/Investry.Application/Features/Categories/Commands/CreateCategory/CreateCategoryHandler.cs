using Investry.Application.Common;
using Investry.Application.Contracts.Persistence;
using Investry.Domain.Entities;
using MediatR;

namespace Investry.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByNameAsync(request.Name);
            if (existingCategory != null)
            {
                return Result<Guid>.Failure(new List<Error> { new Error("Category.Duplicate", "A category with this name already exists.", ErrorType.Conflict) });
            }

            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();

            return Result<Guid>.Success(category.Id);
        }
    }
}
