using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Validation;
using WayFinder.DestinosTuristicosDTOs;
using Xunit;
using Volo.Abp.Authorization;
using Volo.Abp.Security.Claims;
using System.Security.Claims;
using NSubstitute;
using Volo.Abp.Users;
using Volo.Abp.Domain.Repositories;


namespace WayFinder.Calificacion
{
    public class CalificacionAppService_Test 
    {   
      //  private readonly ICalificacionAppService _calificacionAppService; 
      //  private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

     /*   public CalificacionAppService_Test()
        {
            _calificacionAppService = GetRequiredService<ICalificacionAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }
     */
        [Fact]
        // Test para verificar que la calificación se asocia correctamente con el usuario actual
        public async Task CalificarDestinoAsync_Should_Associate_Puntaje_With_Current_User()
        {
           // await Task.Delay(2000);
            // Arrange
            var userId = Guid.NewGuid();
            var currentUser = Substitute.For<ICurrentUser>();
            currentUser.IsAuthenticated.Returns(true);
            currentUser.GetId().Returns(userId);
          
            currentUser.Id.Returns(userId);

            var repo = Substitute.For<IRepository<Calificaciones.Calificacion, Guid>>();
            repo.InsertAsync(Arg.Any<Calificaciones.Calificacion>()).Returns(ci => ci.Arg<WayFinder.Calificaciones.Calificacion>());

            var service = new CalificacionAppService(repo, currentUser);
            var input = new CrearCalificacionDto
            {
                DestinoId = Guid.NewGuid(),
                Puntaje = 5,
                Comentario = "Excelente destino turístico"
            };
            // Act
            var result = await service.CreateAsync(input);
            // Assert
            result.ShouldNotBeNull();
            result.Puntaje.ShouldBe(5);
            result.Comentario.ShouldBe("Excelente destino turístico");
            result.DestinoId.ShouldBe(input.DestinoId);
            result.UserId.ShouldBe(userId);
            /* await WithUnitOfWorkAsync(async () =>
             {
                 var claims = new[]
                 {
                     new Claim(AbpClaimTypes.UserId, testUserId.ToString()),
                     new Claim(AbpClaimTypes.UserName, "testUser")
                 };

                 var identity = new ClaimsIdentity(claims, "TestAuthenticationType");
                 var principal = new ClaimsPrincipal(identity);

                 using (_currentPrincipalAccessor.Change(principal))
                 { 
                     var input = new CrearCalificacionDto
                     {
                         DestinoId = Guid.NewGuid(),
                         Puntaje = 4,
                         Comentario = "Muy buen destino turístico"
                     };

                     // act
                     var result = await _calificacionAppService.CalificarDestinoAsync(input);

                     // assert
                     result.ShouldNotBeNull();
                     result.Puntaje.ShouldBe(4);
                     result.Comentario.ShouldBe("Muy buen destino turístico");
                     result.DestinoId.ShouldBe(input.DestinoId);
                     result.UserId.ShouldBe(testUserId); 
                 }
            
        });*/

        }
        /*
        [Fact]
        // Test para verificar que se lanza una excepción si el usuario no está autenticado
        public async Task CalificarDestinoAsync_Should_Throw_If_Not_Autenticated()
        {
         // Arrange
          var input = new CrearCalificacionDto
            {
                DestinoId = Guid.NewGuid(),
                Puntaje = 5,
            };
            // Act & Assert
            await Should.ThrowAsync<AbpAuthorizationException>(async () =>
            {
                await _calificacionAppService.CalificarDestinoAsync(input);
            });
        } */

    }
        
}
