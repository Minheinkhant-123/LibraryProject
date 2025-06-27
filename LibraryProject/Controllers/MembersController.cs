using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryProject.Models;
using LibraryProject.Core.Interfaces;

public class MembersController : Controller
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _memberService.GetAllAsync();
        return View(members);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Members member)
    {
        var errors = ValidateMember(member);
        if (errors.Count > 0)
        {
            ViewBag.Errors = errors;
            return View(member);
        }

        await _memberService.CreateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound();
        return View(member);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Members member)
    {
        var errors = ValidateMember(member);
        if (errors.Count > 0)
        {
            ViewBag.Errors = errors;
            return View(member);
        }

        var updated = await _memberService.UpdateAsync(member);
        if (!updated)
            return NotFound();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member == null)
            return NotFound();
        return View(member);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _memberService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private List<string> ValidateMember(Members member)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(member.Name))
            errors.Add("Name is required.");

        if (string.IsNullOrWhiteSpace(member.Email))
            errors.Add("Email is required.");
        else if (!IsValidEmail(member.Email))
            errors.Add("Email format is invalid.");

        if (string.IsNullOrWhiteSpace(member.Phone))
            errors.Add("Phone is required.");
        else if (!IsValidPhone(member.Phone))
            errors.Add("Phone format is invalid.");

        if (string.IsNullOrWhiteSpace(member.Address))
            errors.Add("Address is required.");

        if (member.RegistrationDate == default)
            errors.Add("Registration date is required.");

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        bool IsValidPhone(string phone)
        {
            // Example simple validation - adjust as needed
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?\d{7,15}$");
        }

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
