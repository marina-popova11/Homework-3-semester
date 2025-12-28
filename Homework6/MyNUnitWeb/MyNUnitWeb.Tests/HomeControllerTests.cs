// <copyright file="HomeControllerTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnitWeb.Tests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyNUnitWeb.Controllers;
using MyNUnitWeb.Data;

public class HomeControllerTests
{
    [Test]
    public void Test_HomeController_IndexReturnsView()
    {
        var controller = new HomeController(null!, null!);
        var result = controller.Index();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [Test]
    public void Test_HomeController_ReturnsOkResult()
    {
        var controller = new HomeController(null!, null!);
        var result = controller.History();
        Assert.That(result, Is.Not.Null);
    }
}