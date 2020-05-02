var EC = protractor.ExpectedConditions;
const domain = true
? 'https://fantasyhoops.org'
: 'https://localhost:44389';


describe('FantasyHoops login page tests', () =>  {
    beforeAll(() =>  {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/login`);

        var header = element(by.css('.MuiAppBar-root'));
        browser.wait(EC.presenceOf(header), 10000);
    });

    it('should have H1 title', () =>  {
        var h1Title = element(by.css('h1'));
        expect(h1Title.isPresent()).toBe(true);
    });

    it('should have a working login button', () =>  {
        var form = element(by.css('#form'));
        browser.wait(EC.presenceOf(form), 10000);

        var username = element(by.css('#username'));
        username.click();
        username.sendKeys("fantasyhoop");
        
        var password = element(by.css('#password'));
        password.click();
        password.sendKeys("fantasyhoop");
        
        var loginBtn = element(by.css('#login'));
        loginBtn.click();
        
        browser.wait(EC.titleIs('Fantasy Hoops | NBA Fantasy Basketball Game'), 5000);
        
        expect(browser.getCurrentUrl()).toEqual(`${domain}/`);
    });
});

describe('FantasyHoops main page tests', () =>  {
    beforeAll(() =>  {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/`);

        var header = element(by.css('.MuiAppBar-root'));
        browser.wait(EC.presenceOf(header), 10000);
    });
    
    it('should have H1 title', () =>  {
        var h1Title = element(by.css('h1'));
        expect(h1Title.isPresent()).toBe(true);
    });
    
    it('should have a working "Play Now" button', () =>  {
        var playNowBtn = element(by.css('#PlayNowBtn'));
        playNowBtn.click();
        browser.wait(EC.titleIs('Lineup | Fantasy Hoops'), 5000);

        expect(browser.getCurrentUrl()).toEqual(`${domain}/lineup`);
    });
});

describe('FantasyHoops achievements page tests', () =>  {
    beforeAll(() =>  {
        browser.waitForAngularEnabled(false);
        browser.get(`${domain}/achievements`);

        var achievements = element(by.css('.Achievements'));
        browser.wait(EC.presenceOf(achievements), 10000);
    });

    it('should have H1 title', () =>  {
        var h1Title = element(by.css('h1'));
        expect(h1Title.isPresent()).toBe(true);
    });

    it('should have H5 subtitle', () =>  {
        var h1Title = element(by.css('h5'));
        expect(h1Title.isPresent()).toBe(true);
    });
    
    it('should have a clickable achievement card', async () =>  {
        var achievementCard = element.all(by.css('.AchievementCard__Icon')).first();
        achievementCard.click();
        
        var achievementDialog = element(by.css('.AchievementDialog__Paper'));
        browser.wait(EC.presenceOf(achievementDialog), 5000);
        
        expect(achievementDialog.isDisplayed()).toBe(true);
    });
    
    it('should have an achievement card title', async () => {
        var achievementDialogTitle = element(by.css('.AchievementDialog__Title'));
        expect((await achievementDialogTitle.getText()).length).toBeGreaterThan(0);
    });
    
    it('should have an achievement card description', async () => {
        var achievementDialogDescription = element(by.css('.AchievementDialog__Description'));
        expect((await achievementDialogDescription.getText()).length).toBeGreaterThan(0);
    });
    
    it('should be able to close achievement dialog', () =>  {
        var achievementDialog = element(by.css('.AchievementDialog__Paper'));
        var closeBtn = element(by.css('.AchievementDialog__CloseIcon'));
        closeBtn.click();
        
        browser.wait(EC.invisibilityOf(achievementDialog), 5000);

        expect(achievementDialog.isPresent()).toBe(false);
    });
});